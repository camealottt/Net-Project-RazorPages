using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.Chat
{
    public class UserListModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public string? ProfilePicture { get; set; }
        public UserListModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<User> AllUsers { get; set; }

        public void OnGet()
        {
            int? currentUserId = HttpContext.Session.GetInt32("UserID");

            if (currentUserId == null)
            {
                // Redirect to login if user not authenticated
                Response.Redirect("/Session/Login");
                return;
            }

            AllUsers = _context.User
                .Where(u => u.Id != currentUserId)
                .ToList();            
        }

    }
}
