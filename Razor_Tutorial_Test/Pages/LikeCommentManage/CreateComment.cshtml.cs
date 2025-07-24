using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.LikeCommentManage
{
    [BindProperties]
    public class CreateCommentModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public List<Items> ItemsList { get; set; } = new List<Items>();
        public List<CommentRecord> CommentsList { get; set; } = new List<CommentRecord>();
        public List<User> UsersList { get; set; } = new List<User>();

        public int SelectedItemId { get; set; }
        public int SelectedUserId { get; set; }
        public string Content { get; set; }

        public bool ShowSuccessMessage { get; set; }
        public string SuccessMessage { get; set; }

        public CreateCommentModel(ApplicationDbContext db)
        {
            _db = db;
            LoadDropdownData();
        }

        private void LoadDropdownData()
        {
            ItemsList = _db.Items.ToList();
            UsersList = _db.User.ToList();
        }

        public IActionResult OnPostComment()
        {
            if (SelectedItemId == 0 || SelectedUserId == 0 || string.IsNullOrWhiteSpace(Content))
            {
                TempData["ErrorMessage"] = "Please select an item, a user, and enter a comment.";
                LoadDropdownData();
                return Page();
            }

            _db.CommentRecord.Add(new CommentRecord
            {
                Item = SelectedItemId,
                User = SelectedUserId,
                Content = Content,
                CreatedAt = DateTime.Now
            });
            _db.SaveChanges();

            // Reload comments for the selected item
            CommentsList = _db.CommentRecord.Where(c => c.Item == SelectedItemId).ToList();
            LoadDropdownData();

            TempData["AddSuccess"] = "Comment Added successfully!";


            return Page();
        }
    }
}
