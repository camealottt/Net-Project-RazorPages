using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.FollowUnfollowManage
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;


        public List<User> Users { get; set; } // List of all users
        public int AUserId { get; set; } // Selected Follower ID
        public int BUserId { get; set; } // Selected Followed User ID

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            Users = await _db.User.ToListAsync(); // Fetch all users for dropdown
        }

        public async Task<IActionResult> OnPostFollowAsync()
        {
            if (AUserId == BUserId)
            {
                TempData["ErrorMessage"] = "You cannot follow yourself!";
                return RedirectToPage();
            }

            var existingFollow = await _db.FollowRecord
                .FirstOrDefaultAsync(f => f.AUser == AUserId && f.BUser == BUserId);

            if (existingFollow == null)
            {
                var newFollow = new FollowRecord { AUser = AUserId, BUser = BUserId };
                _db.FollowRecord.Add(newFollow);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Followed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "You are already following this user!";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUnfollowAsync()
        {
            var followRecord = await _db.FollowRecord
                .FirstOrDefaultAsync(f => f.AUser == AUserId && f.BUser == BUserId);

            if (followRecord != null)
            {
                _db.FollowRecord.Remove(followRecord);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Unfollowed successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "You are not following this user!";
            }

            return RedirectToPage();
        }
    }
}
