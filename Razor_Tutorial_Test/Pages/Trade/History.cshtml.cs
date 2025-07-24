using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;

namespace Razor_Tutorial_Test.Pages.Trade
{
    public class HistoryModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public HistoryModel(ApplicationDbContext context)
        {
            _context = context;
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
                .Where(o => o.BuyerUserID == currentUserId && o.OrderStatus == "Traded")
                .ToList();

            var ordersSeller = _context.OrderRecord
                .Where(o => o.SellerUserID == currentUserId && o.OrderStatus == "Traded")
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
    }
}
