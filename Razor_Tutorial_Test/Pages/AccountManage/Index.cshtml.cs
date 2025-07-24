using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.AccountManage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<User> Accounts { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var accounts = from a in _db.User
                           select a;

            if (!string.IsNullOrEmpty(SearchString))
            {
                accounts = accounts.Where(s => s.Username.Contains(SearchString));
            }

            Accounts = accounts.ToList();
        }
    }
}