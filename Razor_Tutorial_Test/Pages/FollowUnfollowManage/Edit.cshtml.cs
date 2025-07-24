using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Razor_Tutorial_Test.Pages.FollowUnfollowManage
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public User User { get; set; }

        public List<User> Followers { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            User = await _db.User.FirstOrDefaultAsync(u => u.Id == id);
            if (User == null)
            {
                return NotFound();
            }

            // Get followers list (joined with User)
            Followers = await _db.FollowRecord
                .Where(fr => fr.BUser == id)
                .Join(_db.User,
                      fr => fr.AUser,
                      u => u.Id,
                      (fr, u) => u)   // <--- this returns User
                .ToListAsync();

            return Page();
        }

        // Remove follower
        public async Task<IActionResult> OnPostRemoveFollowerAsync(int followerId, int userId)
        {
            var record = await _db.FollowRecord
                .FirstOrDefaultAsync(fr => fr.AUser == followerId && fr.BUser == userId);

            if (record != null)
            {
                _db.FollowRecord.Remove(record);
                TempData["EditSuccess"] = "Follower Relationship Edited Successfully !";
                await _db.SaveChangesAsync();
            }

            return RedirectToPage(new { id = userId });
        }
    }
}
