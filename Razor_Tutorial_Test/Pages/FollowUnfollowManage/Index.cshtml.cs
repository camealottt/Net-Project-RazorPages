using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.FollowUnfollowManage
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public IEnumerable<User> Accounts { get; set; }
        public Dictionary<int, int> FollowerCounts { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string SearchString { get; set; }

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            // Create query
            var accountsQuery = _db.User.AsQueryable();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(SearchString))
            {
                accountsQuery = accountsQuery.Where(u => u.Username.Contains(SearchString));
            }

            // Fetch filtered accounts
            Accounts = await accountsQuery.ToListAsync();

            // Count followers for each user
            FollowerCounts = await _db.FollowRecord
                .GroupBy(f => f.BUser)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.UserId, x => x.Count);
        }
    }
}