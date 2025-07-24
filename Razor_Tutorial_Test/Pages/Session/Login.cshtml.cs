using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Razor_Tutorial_Test.Pages.Session
{
    [BindProperties]
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public User Accounts { get; set; }

        public LoginModel(ApplicationDbContext db)
        {
            _db = db;
        }
      
        public async Task<IActionResult> OnPost()
        {
            // Check if the user exists in the database based on the provided username
            var user = await _db.User
                .FirstOrDefaultAsync(u => u.Username == Accounts.Username);

            if (user != null && user.PasswordHash == Accounts.PasswordHash)  // Check if password matches
            {
                // Save user data to session
                HttpContext.Session.SetInt32("UserID", user.Id);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetString("Type", user.Type);

                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToPage("/Index"); // Redirect to homepage after successful login
            }

            // If credentials are invalid, return error message
            TempData["ErrorMessage"] = "Invalid username or password!";
            return Page();
        }
    }
}
