using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.ItemManage
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public Items Items { get; set; }

        [BindProperty]
        public List<User> Users { get; set; }

        [BindProperty]
        public List<Category> Category { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task OnGetAsync()
        {
            Users = await _db.User.ToListAsync();
            Category = await _db.Category.ToListAsync();
        }

        public async Task<IActionResult> OnPost()
        {
            

            if (ModelState.IsValid)
            {
                await _db.Items.AddAsync(Items);
                await _db.SaveChangesAsync();
                TempData["AddSuccess"] = "Item created successfully !";

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
