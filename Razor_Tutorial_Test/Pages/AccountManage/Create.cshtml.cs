using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.AccountManage
{
    [BindProperties]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        
        public User Accounts { get; set; }

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPost()
        {            
            if (ModelState.IsValid)
            {
                await _db.User.AddAsync(Accounts);
                await _db.SaveChangesAsync();
                TempData["AddSuccess"] = "User created successfully !";

                return RedirectToPage("Index");
            }
            return Page();
        }
    }
}
