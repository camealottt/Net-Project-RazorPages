using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Collections.Generic;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.UserProfile
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public User UserProfile { get; set; }
        public List<Items> UserItems { get; set; }

        // Declare ItemImages as a list
        public List<ItemImages> ItemImages { get; set; }

        public List<Category> Categories { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserID") == null)
            {
                return RedirectToPage("/Session/Login");
            }

            int userId = HttpContext.Session.GetInt32("UserID").Value;

            UserProfile = _context.User.FirstOrDefault(u => u.Id == userId);
            if (UserProfile == null)
            {
                return NotFound();
            }

            UserItems = _context.Items.Where(i => i.Owner == userId).ToList();

            var itemIds = UserItems.Select(i => i.Id).ToList();

            ItemImages = _context.ItemImages
                                 .Where(img => itemIds.Contains(img.ItemId))
                                 .ToList();

            Categories = _context.Category.ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            var images = _context.ItemImages.Where(img => img.ItemId == id).ToList();

            // Delete physical files
            foreach (var img in images)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "itemimages", Path.GetFileName(img.ImageUrl));
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // Remove image records from DB
            _context.ItemImages.RemoveRange(images);

            // Remove item
            _context.Items.Remove(item);

            await _context.SaveChangesAsync();

            TempData["DeleteSuccess"] = "Item deleted successfully!";

            return RedirectToPage();
        }


    }
}
