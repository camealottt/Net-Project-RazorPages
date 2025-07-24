using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Pages.Session
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public User Accounts { get; set; }

        public RegisterModel(ApplicationDbContext db)
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
                var existingUser = await _db.User
                    .FirstOrDefaultAsync(u => u.Username == Accounts.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Accounts.Username", "Username is already taken.");
                    return Page(); 
                }

                await _db.User.AddAsync(Accounts);
                await _db.SaveChangesAsync();
                TempData["SuccessMessage"] = "User created successfully!";

                return RedirectToPage("/Index"); 
            }
            TempData["ErrorMessage"] = "Please Insert All Information.";

            return Page(); 
        }

    }
}
