using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Razor_Tutorial_Test.Pages.AccountManage
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public User Accounts { get; set; }

        [BindProperty]
        public IFormFile? ProfilePicture { get; set; }


        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int id)
        {
            Accounts = _db.User.Find(id);
        }

        [BindProperty]
        public string? CroppedImage { get; set; } // Stores Base64 image

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

            // Only require CroppedImage if a new image is uploaded
            if (ProfilePicture != null && string.IsNullOrEmpty(CroppedImage))
            {
                ModelState.AddModelError("CroppedImage", "Please crop the image before updating.");
                return Page();
            }

            // Process cropped image if provided
            if (!string.IsNullOrEmpty(CroppedImage))
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                string uniqueFileName = $"{Guid.NewGuid()}.png";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Check if the user has an old profile picture and delete it
                if (!string.IsNullOrEmpty(existingAccount.ProfilePicture))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingAccount.ProfilePicture.TrimStart('/'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Convert Base64 to Image File
                byte[] imageBytes = Convert.FromBase64String(CroppedImage.Split(',')[1]);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Update Profile Picture Path
                existingAccount.ProfilePicture = "/images/" + uniqueFileName;
            }

            // Update other user details
            existingAccount.Username = Accounts.Username;
            existingAccount.Email = Accounts.Email;
            existingAccount.PasswordHash = Accounts.PasswordHash;

            _db.User.Update(existingAccount);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
