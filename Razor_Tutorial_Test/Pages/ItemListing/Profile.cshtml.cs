using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.ItemListing
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User UserProfile { get; set; }

        public List<Items> UserItems { get; set; } = new();
        public List<ItemImages> ItemImages { get; set; } = new();
        public List<Category> Categories { get; set; } = new();

        public Dictionary<int, List<CommentDisplayModel>> CommentsDict { get; set; } = new();

        public int TotalFollowers { get; set; }
        public int TotalFollowing { get; set; }
        public bool IsFollowing { get; set; }

        public List<ItemDisplayModel> UserItemsDisplay { get; set; } = new();

        public class CommentDisplayModel
        {
            public string Username { get; set; }
            public string? ProfilePicture { get; set; }
            public string CommentText { get; set; }
        }

        public class ItemDisplayModel
        {
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int LikeCount { get; set; }
            public bool UserLiked { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserID");

            UserProfile = await _context.User.FindAsync(id);
            if (UserProfile == null) return NotFound();

            UserItems = await _context.Items.Where(i => i.Owner == id).ToListAsync();
            ItemImages = await _context.ItemImages.ToListAsync();
            Categories = await _context.Category.ToListAsync();

            TotalFollowers = await _context.FollowRecord.CountAsync(f => f.BUser == id);
            TotalFollowing = await _context.FollowRecord.CountAsync(f => f.AUser == id);

            IsFollowing = sessionUserId != null && await _context.FollowRecord.AnyAsync(f => f.AUser == sessionUserId && f.BUser == id);

            foreach (var item in UserItems)
            {
                var itemDisplay = new ItemDisplayModel
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    LikeCount = await _context.LikeRecord.CountAsync(l => l.Item == item.Id),
                    UserLiked = sessionUserId != null && await _context.LikeRecord.AnyAsync(l => l.Item == item.Id && l.User == sessionUserId)
                };
                UserItemsDisplay.Add(itemDisplay);

                var comments = await _context.CommentRecord
                    .Where(c => c.Item == item.Id)
                    .Join(_context.User,
                        c => c.User,
                        u => u.Id,
                        (c, u) => new CommentDisplayModel
                        {
                            Username = u.Username,
                            ProfilePicture = u.ProfilePicture,
                            CommentText = c.Content
                        }).ToListAsync();

                CommentsDict[item.Id] = comments;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostFollowAsync(int id)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserID");

            if (sessionUserId == null || sessionUserId == id)
            {
                TempData["ErrorMessage"] = "You cannot follow yourself.";
                return RedirectToPage(new { id });
            }

            var existing = await _context.FollowRecord
                .FirstOrDefaultAsync(f => f.AUser == sessionUserId && f.BUser == id);

            if (existing != null)
            {
                _context.FollowRecord.Remove(existing);
                TempData["SuccessMessage"] = "You unfollowed the user!";
            }
            else
            {
                _context.FollowRecord.Add(new FollowRecord
                {
                    AUser = sessionUserId.Value,
                    BUser = id
                });
                TempData["SuccessMessage"] = "You followed the user!";
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostLikeAsync(int id, int userId)
        {
            var sessionUserId = HttpContext.Session.GetInt32("UserID");
            if (sessionUserId == null)
            {
                TempData["ErrorMessage"] = "You need to login first!";
                return RedirectToPage("Profile", new { id = userId });
            }

            var existing = await _context.LikeRecord
                .FirstOrDefaultAsync(l => l.Item == id && l.User == sessionUserId);

            if (existing != null)
            {
                _context.LikeRecord.Remove(existing);
                TempData["SuccessMessage"] = "You unliked the item!";
            }
            else
            {
                _context.LikeRecord.Add(new LikeRecord
                {
                    Item = id,
                    User = sessionUserId.Value
                });
                TempData["SuccessMessage"] = "You liked the item!";
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("Profile", new { id = userId });
        }


        public async Task<IActionResult> OnPostAddCommentAsync(int id, string CommentText, int userId)
        {
            var sessionId = HttpContext.Session.GetInt32("UserID");
            if (sessionId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to comment.";
                return RedirectToPage("Profile", new { id = userId });
            }

            if (string.IsNullOrWhiteSpace(CommentText))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToPage("Profile", new { id = userId });
            }

            _context.CommentRecord.Add(new CommentRecord
            {
                User = sessionId.Value,
                Item = id,
                Content = CommentText
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Comment added successfully!";
            return RedirectToPage("Profile", new { id = userId });
        }
    }
}
