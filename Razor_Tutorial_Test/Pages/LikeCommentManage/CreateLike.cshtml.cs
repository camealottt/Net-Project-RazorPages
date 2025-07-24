using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.LikeCommentManage
{
    [BindProperties]
    public class CreateLikeModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public List<Items> ItemsList { get; set; } = new List<Items>();
        public List<User> UsersList { get; set; } = new List<User>();

        public int SelectedItemId { get; set; }
        public int SelectedUserId { get; set; }

        public bool ShowSuccessMessage { get; set; }
        public string SuccessMessage { get; set; }

        public CreateLikeModel(ApplicationDbContext db)
        {
            _db = db;
            LoadDropdownData();
        }

        private void LoadDropdownData()
        {
            ItemsList = _db.Items.ToList();
            UsersList = _db.User.ToList();
        }

        public IActionResult OnPostLike()
        {
            if (SelectedItemId == 0 || SelectedUserId == 0)
            {
                TempData["ErrorMessage"] = "Please select an item and a user.";
                LoadDropdownData();
                return Page();
            }

            var existingLike = _db.LikeRecord.FirstOrDefault(l => l.Item == SelectedItemId && l.User == SelectedUserId);

            if (existingLike != null)
            {
                TempData["ErrorMessage"] = "You have already liked this item.";
            }
            else
            {
                _db.LikeRecord.Add(new LikeRecord { Item = SelectedItemId, User = SelectedUserId, CreatedAt = DateTime.Now });
                _db.SaveChanges();
                TempData["SuccessMessage"] = "You have liked this item!";
            }

            LoadDropdownData();
            return Page();
        }
    }
}
