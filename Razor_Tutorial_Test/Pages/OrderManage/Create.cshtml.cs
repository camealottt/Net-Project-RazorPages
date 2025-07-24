using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.OrderManage
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public OrderRecord OrderRecord { get; set; }

        public List<SelectListItem> UserList { get; set; }
        public List<SelectListItem> OfferedItems { get; set; } = new();
        public List<SelectListItem> RequestedItems { get; set; } = new();

        public bool ShowForm { get; set; }

        public void OnGet(int? buyerId, int? sellerId)
        {
            LoadUsers();

            if (buyerId.HasValue && sellerId.HasValue)
            {
                ShowForm = true;
                OrderRecord = new OrderRecord
                {
                    BuyerUserID = buyerId.Value,
                    SellerUserID = sellerId.Value
                };

                OfferedItems = _db.Items
                    .Where(i => i.Owner == buyerId)
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Name
                    }).ToList();

                RequestedItems = _db.Items
                    .Where(i => i.Owner == sellerId)
                    .Select(i => new SelectListItem
                    {
                        Value = i.Id.ToString(),
                        Text = i.Name
                    }).ToList();
            }
        }

        public IActionResult OnPost()
        {
            LoadUsers();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            OrderRecord.OrderStatus = "Pending";
            OrderRecord.CreatedAt = DateTime.Now;

            _db.OrderRecord.Add(OrderRecord);
            _db.SaveChanges();
            TempData["AddSuccess"] = "Order Added successfully !";

            return RedirectToPage("/OrderManage/Index");
        }

        private void LoadUsers()
        {
            UserList = _db.User
                .Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.Username
                }).ToList();
        }
    }

}
