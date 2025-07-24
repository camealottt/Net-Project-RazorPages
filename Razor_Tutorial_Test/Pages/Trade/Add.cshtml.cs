using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Razor_Tutorial_Test.Services;

namespace Razor_Tutorial_Test.Pages.Trade
{
    public class AddModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly NotificationService _notificationService;
        public AddModel(ApplicationDbContext db, NotificationService notificationService)
        {
            _db = db;
            _notificationService = notificationService;
        }

        [BindProperty]
        public OrderRecord OrderRecord { get; set; } = new();

        [BindProperty]
        [Range(0, double.MaxValue, ErrorMessage = "Offer price cannot be negative.")]
        public decimal? MoneyOfferedInput { get; set; }
        public Items? BuyerItem { get; set; }

        public Items? SellerItem { get; set; }
        public string SellerItemCategoryName { get; set; } // Added property for category name
        public List<ItemImages> SellerItemImages { get; set; } = new();
        public User? SellerUser { get; set; }
        [BindProperty]
        public int SellerItemId { get; set; }
        public List<Items> BuyerItems { get; set; } = new();
        public Dictionary<int, List<ItemImages>> BuyerItemImages { get; set; } = new();
        public User? BuyerUser { get; set; }
        public string SelectedBuyerItemCategoryName { get; set; } // Added property for category name

        [BindProperty]
        public int? SelectedBuyerItemId { get; set; }

        public Items? SelectedBuyerItem { get; set; }
        public List<ItemImages> SelectedBuyerItemImages { get; set; } = new();

        // Added: Item Trade History
        public List<TradeHistoryViewModel> ItemTradeHistory { get; set; } = new();

        public IActionResult OnGet(int? itemId, int? buyerItemId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToPage("/Session/Login");

            BuyerUser = _db.User.FirstOrDefault(u => u.Id == userId);
            if (BuyerUser == null)
                return RedirectToPage("/Session/Login");

            if (itemId == null)
                return RedirectToPage("/ItemListing/Index");

            // Load seller item
            SellerItem = _db.Items
                .Where(i => i.Id == itemId)
                .FirstOrDefault();

            if (SellerItem == null)
                return NotFound();

            SellerItemId = SellerItem.Id;

            // Fetch the category name separately based on the category ID from the Item
            var sellerCategory = _db.Category
                .Where(c => c.Id == SellerItem.Category)
                .FirstOrDefault();
            SellerItemCategoryName = sellerCategory?.Name ?? "Unknown";

            // Load images associated with the seller's item
            SellerItemImages = _db.ItemImages.Where(img => img.ItemId == itemId).ToList();

            // Fetch the seller user
            SellerUser = _db.User.FirstOrDefault(u => u.Id == SellerItem.Owner);

            // Load the buyer's items
            BuyerItems = _db.Items
                .Where(i => i.Owner == userId && i.Status != "Traded" && i.Status != "Unavailable")
                .ToList();

            // Load the images for each buyer item
            foreach (var item in BuyerItems)
            {
                BuyerItemImages[item.Id] = _db.ItemImages.Where(img => img.ItemId == item.Id).ToList();
            }

            // If a buyer item is selected
            if (buyerItemId.HasValue)
            {
                SelectedBuyerItemId = buyerItemId;
                SelectedBuyerItem = _db.Items.FirstOrDefault(i => i.Id == buyerItemId && i.Owner == userId);
                if (SelectedBuyerItem != null)
                {
                    SelectedBuyerItemImages = _db.ItemImages.Where(img => img.ItemId == SelectedBuyerItem.Id).ToList();

                    // Fetch the category name for the selected buyer item
                    var buyerCategory = _db.Category
                        .Where(c => c.Id == SelectedBuyerItem.Category)
                        .FirstOrDefault();
                    SelectedBuyerItemCategoryName = buyerCategory?.Name ?? "Unknown"; // Set default if category is not found
                }
            }

            // Load the item's trade history
            LoadItemTradeHistory(itemId.Value);

            return Page();
        }

        // Added: Method to load trade history for the item
        private void LoadItemTradeHistory(int itemId)
        {
            // Get all trade records where this item was requested
            var tradeRecords = _db.OrderRecord
                .Where(o => o.RequestedItemID == itemId)
                .OrderByDescending(o => o.CreatedAt)
                .ToList();

            ItemTradeHistory = new List<TradeHistoryViewModel>();

            foreach (var record in tradeRecords)
            {
                var historyItem = new TradeHistoryViewModel
                {
                    OrderId = record.Id,
                    OrderStatus = record.OrderStatus,
                    CreatedAt = record.CreatedAt
                };

                // Get offered item details if it exists
                if (record.OfferedItemID.HasValue)
                {
                    var offeredItem = _db.Items.FirstOrDefault(i => i.Id == record.OfferedItemID.Value);
                    if (offeredItem != null)
                    {
                        historyItem.OfferedItemName = offeredItem.Name;

                        // Get the first image of the offered item
                        var offeredItemImage = _db.ItemImages
                            .Where(img => img.ItemId == offeredItem.Id)
                            .FirstOrDefault();

                        historyItem.OfferedItemImageUrl = offeredItemImage?.ImageUrl;
                    }
                }

                // Add money offered info if available
                historyItem.MoneyOffered = record.MoneyOffered;

                ItemTradeHistory.Add(historyItem);
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
                return RedirectToPage("/Session/Login");

            if (!ModelState.IsValid)
                return Page();

            OrderRecord.BuyerUserID = userId.Value;
            OrderRecord.OfferedItemID = SelectedBuyerItemId;
            OrderRecord.MoneyOffered = MoneyOfferedInput;
            OrderRecord.OrderStatus = "Pending";
            OrderRecord.CreatedAt = DateTime.Now;

            SellerItem = _db.Items.FirstOrDefault(i => i.Id == SellerItemId);
            if (SellerItem == null)
                return NotFound();

            OrderRecord.RequestedItemID = SellerItem.Id;
            OrderRecord.SellerUserID = SellerItem.Owner;

            _db.OrderRecord.Add(OrderRecord);
            _db.SaveChanges();

            // Create notifications using the service
            await _notificationService.CreateNotificationAsync(
                SellerItem.Owner,
                "A new trade offer has been submitted for your item.",
                false); // Don't send update yet

            await _notificationService.CreateNotificationAsync(
                userId.Value,
                "Your offer has been submitted.",
                false); // Don't send update yet

            // Send the updates now
            await _notificationService.SendUnreadCountUpdateAsync(userId.Value);
            await _notificationService.SendUnreadCountUpdateAsync(SellerItem.Owner);

            TempData["SuccessMessage"] = "Trade offer submitted successfully!";
            return RedirectToPage("/Trade/Index");
        }
    }

    // Added: View model for trade history items
    public class TradeHistoryViewModel
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string OfferedItemName { get; set; }
        public string OfferedItemImageUrl { get; set; }
        public decimal? MoneyOffered { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}