using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.LikeCommentManage
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public int TotalLikes { get; set; }
        public int TotalComments { get; set; }
        public List<UserLikeViewModel> LikeList { get; set; } = new();
        public List<UserCommentViewModel> CommentList { get; set; } = new();
        public int ItemId { get; set; }

        public class UserLikeViewModel
        {
            public int LikeId { get; set; }
            public string Username { get; set; }
            public DateTime LikedAt { get; set; }
        }

        public class UserCommentViewModel
        {
            public int CommentId { get; set; }
            public string Username { get; set; }
            public string Content { get; set; }
            public DateTime CommentedAt { get; set; }
        }

        public IActionResult OnGet(int id)
        {
            ItemId = id;

            // Fetch total likes & comments for the selected item
            TotalLikes = _db.LikeRecord.Count(l => l.Item == id);
            TotalComments = _db.CommentRecord.Count(c => c.Item == id);

            // Retrieve users who liked the item
            LikeList = _db.LikeRecord
                .Where(l => l.Item == id)
                .Join(_db.User, l => l.User, u => u.Id, (l, u) => new UserLikeViewModel
                {
                    LikeId = l.Id,
                    Username = u.Username,
                    LikedAt = l.CreatedAt
                }).ToList();

            // Retrieve users who commented on the item
            CommentList = _db.CommentRecord
                .Where(c => c.Item == id)
                .Join(_db.User, c => c.User, u => u.Id, (c, u) => new UserCommentViewModel
                {
                    CommentId = c.Id,
                    Username = u.Username,
                    Content = c.Content,
                    CommentedAt = c.CreatedAt
                }).ToList();

            return Page();
        }

        public IActionResult OnPostDeleteLike(int likeId, int itemId)
        {
            var like = _db.LikeRecord.FirstOrDefault(l => l.Id == likeId && l.Item == itemId);
            if (like != null)
            {
                _db.LikeRecord.Remove(like);
                _db.SaveChanges();
                TempData["DeleteSuccess"] = "Like Recorded deleted successfully !";

            }
            return RedirectToPage(new { id = itemId });
        }

        public IActionResult OnPostDeleteComment(int commentId, int itemId)
        {
            var comment = _db.CommentRecord.FirstOrDefault(c => c.Id == commentId && c.Item == itemId);
            if (comment != null)
            {
                _db.CommentRecord.Remove(comment);
                _db.SaveChanges();
                TempData["DeleteSuccess"] = "Comment deleted successfully !";

            }
            return RedirectToPage(new { id = itemId });
        }
    }
}
