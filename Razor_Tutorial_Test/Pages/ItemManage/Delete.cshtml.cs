using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.ItemManage
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public Items Items { get; set; }

        public DeleteModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet(int id)
        {
            Items = _db.Items.Find(id);
        }

        public async Task<IActionResult> OnPost()
        {
            var itemFromDb = _db.Items.Find(Items.Id);
            if (itemFromDb != null)
            {
                var images = _db.ItemImages.Where(img => img.ItemId == Items.Id).ToList();

                foreach (var img in images)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "itemimages", Path.GetFileName(img.ImageUrl));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _db.ItemImages.RemoveRange(images);

                _db.Items.Remove(itemFromDb);

                await _db.SaveChangesAsync(); 

                TempData["DeleteSuccess"] = "Item deleted successfully!";
                return RedirectToPage("Index");
            }
            return Page();
        }

    }
}
