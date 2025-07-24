using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using Razor_Tutorial_Test.Services;

namespace Razor_Tutorial_Test.Pages.Trade
{
    public class ItemDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public ItemDetailsModel(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [BindProperty(SupportsGet = true)]
        public string From { get; set; }

        public OrderRecordedDisplayModel OrderDetails { get; set; }

        public class OrderRecordedDisplayModel
        {
            // Buyer Information
            public int BuyerUserID { get; set; }
            public string BuyerUsername { get; set; }
            public string BuyerProfilePictureUrl { get; set; }
            public string OfferedItemName { get; set; }
            public string OfferedItemDescription { get; set; }
            public string OfferedItemCategory { get; set; }
            public decimal OfferedItemPrice { get; set; }
            public List<ItemImages> BuyerItemImages { get; set; } = new List<ItemImages>();

            // Seller Information
            public int SellerUserID { get; set; }
            public string SellerUsername { get; set; }
            public string SellerProfilePictureUrl { get; set; }
            public string RequestedItemName { get; set; }
            public string RequestedItemDescription { get; set; }
            public string RequestedItemCategory { get; set; }
            public decimal RequestedItemPrice { get; set; }
            public List<ItemImages> SellerItemImages { get; set; } = new List<ItemImages>();

            // Order Information
            public int Id { get; set; }
            public decimal? MoneyOffered { get; set; }
            public DateTime CreatedAt { get; set; }
            public string OrderStatus { get; set; }

            // Chat purpose
            public int? CurrentUserId { get; set; }
        }
        public async Task<IActionResult> OnGetAsync(int id, string from)
        {
            From = from;

            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToPage("/Session/Login");

            var order = await _context.OrderRecord
                .Where(o => o.Id == id)
                .Select(o => new
                {
                    o.OfferedItemID,
                    o.RequestedItemID,
                    o.MoneyOffered,
                    o.CreatedAt,
                    o.OrderStatus,

                    Buyer = _context.User.FirstOrDefault(u => u.Id == o.BuyerUserID),
                    Seller = _context.User.FirstOrDefault(u => u.Id == o.SellerUserID),

                    OfferedItem = _context.Items.FirstOrDefault(i => i.Id == o.OfferedItemID),
                    RequestedItem = _context.Items.FirstOrDefault(i => i.Id == o.RequestedItemID),
                })
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            var offeredCategory = await _context.Category
                .Where(c => c.Id == order.OfferedItem.Category)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            var requestedCategory = await _context.Category
                .Where(c => c.Id == order.RequestedItem.Category)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();

            var offeredImages = _context.ItemImages.Where(img => img.ItemId == order.OfferedItemID).ToList();

            var requestedImages = _context.ItemImages.Where(img => img.ItemId == order.RequestedItemID).ToList();

            // for chat purpose
            int? currentUserId = HttpContext.Session.GetInt32("UserID");

            OrderDetails = new OrderRecordedDisplayModel
            {
                // Buyer
                BuyerUserID = order.Buyer.Id,
                BuyerUsername = order.Buyer.Username,
                BuyerProfilePictureUrl = order.Buyer.ProfilePicture,
                OfferedItemName = order.OfferedItem.Name,
                OfferedItemDescription = order.OfferedItem.Description,
                OfferedItemCategory = offeredCategory,
                OfferedItemPrice = order.OfferedItem.Price,
                BuyerItemImages = offeredImages,

                // Seller
                SellerUserID = order.Seller.Id,
                SellerUsername = order.Seller.Username,
                SellerProfilePictureUrl = order.Seller.ProfilePicture,
                RequestedItemName = order.RequestedItem.Name,
                RequestedItemDescription = order.RequestedItem.Description,
                RequestedItemCategory = requestedCategory,
                RequestedItemPrice = order.RequestedItem.Price,
                SellerItemImages = requestedImages,

                // Order
                Id = id,
                MoneyOffered = order.MoneyOffered,
                CreatedAt = order.CreatedAt,
                OrderStatus = order.OrderStatus,

                // Chat Purpose
                CurrentUserId = currentUserId

            };

            return Page();
        }
        public async Task<IActionResult> OnPostCancelOrderAsync(int id)
        {         
            var order = await _context.OrderRecord.FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                _context.OrderRecord.Remove(order);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage($"/Trade/{From ?? "Index"}");
        }


        public async Task<IActionResult> OnPostAcceptOfferAsync(int orderID)
        {
            // Find the order
            var order = await _context.OrderRecord.FindAsync(orderID);
            if (order != null)
            {
                // Update order status
                order.OrderStatus = "Traded";
                // Update status of Requested Item (always exists)
                var requestedItem = await _context.Items.FindAsync(order.RequestedItemID);
                if (requestedItem != null)
                {
                    requestedItem.Status = "Traded";
                }
                // Update status of Offered Item (nullable)
                if (order.OfferedItemID.HasValue)
                {
                    var offeredItem = await _context.Items.FindAsync(order.OfferedItemID.Value);
                    if (offeredItem != null)
                    {
                        offeredItem.Status = "Traded";
                    }
                }
                // Create notifications
                await _notificationService.CreateNotificationAsync(
                    order.BuyerUserID,
                    $"Trade completed! Your trade request for item ID {order.RequestedItemID} was successful.",
                    false); // Don't send update yet
                await _notificationService.CreateNotificationAsync(
                    order.SellerUserID,
                    $"Trade completed! You successfully traded your item ID {order.RequestedItemID}.",
                    false); // Don't send update yet
                            // Save all changes
                await _context.SaveChangesAsync();
                // Now send updates to both users
                await _notificationService.SendUnreadCountUpdateAsync(order.BuyerUserID);
                await _notificationService.SendUnreadCountUpdateAsync(order.SellerUserID);
            }
            return RedirectToPage($"/Trade/{From ?? "Index"}");
        }
    }
}
