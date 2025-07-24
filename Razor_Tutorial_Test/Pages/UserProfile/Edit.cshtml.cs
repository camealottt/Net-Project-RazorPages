using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.UserProfile
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public User Accounts { get; set; } = new();

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }

        [BindProperty]
        public string? CroppedImage { get; set; }

        public void OnGet(int id)
        {
            var user = _db.User.Find(id);
            if (user != null)
            {
                Accounts = user;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingAccount = await _db.User.FindAsync(Accounts.Id);
            if (existingAccount == null)
            {
                return NotFound();
            }

            if (ProfilePicture != null && string.IsNullOrEmpty(CroppedImage))
            {
                ModelState.AddModelError("CroppedImage", "Please crop the image before updating.");
                return Page();
            }

            if (!string.IsNullOrEmpty(CroppedImage))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                Directory.CreateDirectory(uploadsFolder); // Ensure folder exists

                string uniqueFileName = $"{Guid.NewGuid()}.png";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Delete old image if exists
                if (!string.IsNullOrEmpty(existingAccount.ProfilePicture))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingAccount.ProfilePicture.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                byte[] imageBytes = Convert.FromBase64String(CroppedImage.Split(',')[1]);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                existingAccount.ProfilePicture = "/images/" + uniqueFileName;
            }

            existingAccount.Username = Accounts.Username;
            existingAccount.Email = Accounts.Email;
            existingAccount.PasswordHash = Accounts.PasswordHash;
            existingAccount.Bio = Accounts.Bio;

            _db.User.Update(existingAccount);
            await _db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile Edited Successfully!";

            return RedirectToPage("Index");
        }
    }
}
