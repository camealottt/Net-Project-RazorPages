using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.OrderManage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<OrderRecord> OrderRecord { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var orders = _db.OrderRecord.AsQueryable();

            if (!string.IsNullOrEmpty(SearchString))
            {
                // Filter orders by ID or status
                orders = orders.Where(o =>
                    o.Id.ToString().Contains(SearchString) ||
                    (o.OrderStatus != null && o.OrderStatus.Contains(SearchString))
                );
            }

            OrderRecord = orders.ToList();
        }
    }
}