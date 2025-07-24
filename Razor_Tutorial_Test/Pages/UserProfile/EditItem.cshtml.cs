using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.UserProfile
{
    [BindProperties]
    public class EditItemModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public Items Item { get; set; }

        [BindProperty]
        public List<ItemImages> ItemImages { get; set; }

        public EditItemModel(ApplicationDbContext db)
        {
            _db = db;
        }
        [BindProperty]
        public List<User> Users { get; set; }

        [BindProperty]
        public List<Category> Category { get; set; }

        public async Task OnGetAsync(int id)
        {
            Item = await _db.Items.FindAsync(id);
            if (Item == null)
            {
                NotFound();
            }

            Users = await _db.User.ToListAsync();
            Category = await _db.Category.ToListAsync();
            ItemImages = await _db.ItemImages.Where(i => i.ItemId == id).ToListAsync();
        }


        public async Task<IActionResult> OnPostEditAsync()
        {
            if (ModelState.IsValid)
            {
                _db.Items.Update(Item);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Items Edited Successfully!";

                return RedirectToPage("Index");
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUploadCroppedImageAsync(int itemId, string croppedImage)
        {
            if (string.IsNullOrEmpty(croppedImage))
            {
                TempData["ErrorMessage"] = "No image provided.";
                return RedirectToPage(new { id = itemId });
            }

            try
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/itemimages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = $"{Guid.NewGuid()}.png";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Convert Base64 to Image File
                byte[] imageBytes = Convert.FromBase64String(croppedImage.Split(',')[1]);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Save image details to database
                var newImage = new ItemImages
                {
                    ImageUrl = "/itemimages/" + uniqueFileName,
                    ItemId = itemId
                };

                _db.ItemImages.Add(newImage);
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Image uploaded successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            return RedirectToPage(new { id = itemId });
        }


        public async Task<IActionResult> OnPostDeleteImageAsync(int imageId, int itemId)
        {
            var image = await _db.ItemImages.FindAsync(imageId);
            if (image == null)
            {
                TempData["ErrorMessage"] = "Image not found.";
                return Page(); // Stay on the same page
            }

            try
            {
                // Construct the correct file path
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));

                // Ensure file exists before deleting
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Remove image from the database
                _db.ItemImages.Remove(image);
                await _db.SaveChangesAsync();

                // Prevent validation errors from appearing
                ModelState.Clear();

                TempData["DeleteSuccess"] = "Image deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
            }

            // Reload the images after deletion
            ItemImages = await _db.ItemImages.Where(i => i.ItemId == itemId).ToListAsync();
            await OnGetAsync(itemId);
            return Page(); // Prevents unintended redirection
        }
    }
}
