using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.CategoriesManage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<Category> Categories { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var categories = from c in _db.Category select c;

            if (!string.IsNullOrEmpty(SearchString))
            {
                categories = categories.Where(c => c.Name.Contains(SearchString));
            }

            Categories = categories.ToList();
        }
    }
}