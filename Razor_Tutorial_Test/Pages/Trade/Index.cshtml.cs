using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Razor_Tutorial_Test.Services;

namespace Razor_Tutorial_Test.Pages.Trade
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;

        public IndexModel(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;

        }

        public class OrderDisplayViewModel
        {
            public int OrderId { get; set; }
            public string? OrderStatus { get; set; }
            public string CreatedAtFormatted => CreatedAt.ToString("dd/MM/yyyy");

            public string BuyerUsername { get; set; }
            public string BuyerProfileUrl { get; set; }

            public string SellerUsername { get; set; }
            public string SellerProfileUrl { get; set; }

            public string? OfferedItemName { get; set; }
            public string? OfferedItemImageUrl { get; set; }

            public string RequestedItemName { get; set; }
            public string RequestedItemImageUrl { get; set; }

            public DateTime CreatedAt { get; set; }
        }

        public List<OrderDisplayViewModel> OrdersByBuyer { get; set; } = new();
        public List<OrderDisplayViewModel> OrdersToSeller { get; set; } = new();

        public IActionResult OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserID");

            if (currentUserId == null)
                return RedirectToPage("/Session/Login");

            var users = _context.User.ToList();
            var items = _context.Items.ToList();
            var images = _context.ItemImages.ToList();

            // Filter orders where the status is "Pending"
            var ordersBuyer = _context.OrderRecord
                .Where(o => o.BuyerUserID == currentUserId && o.OrderStatus == "Pending")
                .ToList();

            var ordersSeller = _context.OrderRecord
                .Where(o => o.SellerUserID == currentUserId && o.OrderStatus == "Pending")
                .ToList();

            OrdersByBuyer = ordersBuyer.Select(o => new OrderDisplayViewModel
            {
                OrderId = o.Id,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt,

                BuyerUsername = users.FirstOrDefault(u => u.Id == o.BuyerUserID)?.Username,
                BuyerProfileUrl = users.FirstOrDefault(u => u.Id == o.BuyerUserID)?.ProfilePicture ?? "images/default-profile.png",

                SellerUsername = users.FirstOrDefault(u => u.Id == o.SellerUserID)?.Username,
                SellerProfileUrl = users.FirstOrDefault(u => u.Id == o.SellerUserID)?.ProfilePicture ?? "images/default-profile.png",

                OfferedItemName = items.FirstOrDefault(i => i.Id == o.OfferedItemID)?.Name,
                OfferedItemImageUrl = images.FirstOrDefault(img => img.ItemId == o.OfferedItemID)?.ImageUrl ?? "itemimages/no-image.png",

                RequestedItemName = items.FirstOrDefault(i => i.Id == o.RequestedItemID)?.Name,
                RequestedItemImageUrl = images.FirstOrDefault(img => img.ItemId == o.RequestedItemID)?.ImageUrl ?? "itemimages/no-image.png"

            }).ToList();

            OrdersToSeller = ordersSeller.Select(o => new OrderDisplayViewModel
            {
                OrderId = o.Id,
                OrderStatus = o.OrderStatus,
                CreatedAt = o.CreatedAt,

                BuyerUsername = users.FirstOrDefault(u => u.Id == o.BuyerUserID)?.Username,
                BuyerProfileUrl = users.FirstOrDefault(u => u.Id == o.BuyerUserID)?.ProfilePicture ?? "images/default-profile.png",

                SellerUsername = users.FirstOrDefault(u => u.Id == o.SellerUserID)?.Username,
                SellerProfileUrl = users.FirstOrDefault(u => u.Id == o.SellerUserID)?.ProfilePicture ?? "images/default-profile.png",

                OfferedItemName = items.FirstOrDefault(i => i.Id == o.OfferedItemID)?.Name,
                OfferedItemImageUrl = images.FirstOrDefault(img => img.ItemId == o.OfferedItemID)?.ImageUrl ?? "itemimages/no-image.png",

                RequestedItemName = items.FirstOrDefault(i => i.Id == o.RequestedItemID)?.Name,
                RequestedItemImageUrl = images.FirstOrDefault(img => img.ItemId == o.RequestedItemID)?.ImageUrl ?? "itemimages/no-image.png"
            }).ToList();

            return Page();
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

            return RedirectToPage();
        }
    }
}
