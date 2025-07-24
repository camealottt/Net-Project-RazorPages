using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.AccountManage
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public User Accounts { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int id)
        {
            Accounts = _db.User.Find(id);
        }

        public async Task<IActionResult> OnPost()
        {          
            var accountFromDb = _db.User.Find(Accounts.Id);
            if (accountFromDb != null)
            {
                // Check if the user has a profile picture
                if (!string.IsNullOrEmpty(accountFromDb.ProfilePicture))
                {
                    // Get the full path of the image file
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", accountFromDb.ProfilePicture.TrimStart('/'));

                    // Check if the file exists before deleting
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                // Remove user from the database
                _db.User.Remove(accountFromDb);
                await _db.SaveChangesAsync();
                TempData["DeleteSuccess"] = "Account deleted successfully!";

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
