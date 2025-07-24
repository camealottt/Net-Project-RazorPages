using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.OrderManage
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public OrderRecord OrderRecord { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int id)
        {
            OrderRecord = _db.OrderRecord.Find(id);
        }

        public async Task<IActionResult> OnPost()
        {          
            var orderFromDb = _db.OrderRecord.Find(OrderRecord.Id);
            if (orderFromDb != null)
            {
                _db.OrderRecord.Remove(orderFromDb);
                await _db.SaveChangesAsync();
                TempData["DeleteSuccess"] = "Order deleted successfully !";

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
