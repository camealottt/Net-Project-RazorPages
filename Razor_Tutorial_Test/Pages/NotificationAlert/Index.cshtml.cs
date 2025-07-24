using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Collections.Generic;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.NotificationAlert
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Notification> UserNotifications { get; set; } = new List<Notification>();

        public IActionResult OnGet()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToPage("/Session/Login"); // Redirect if not logged in
            }

            UserNotifications = _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();

            return Page();
        }
        public async Task<IActionResult> OnPostDisableUnreadAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToPage("/Session/Login");
            }

            var notification = await _context.Notifications.FindAsync(id);

            if (notification != null && notification.UserId == userId)
            {
                notification.IsRead = true; 
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                return RedirectToPage("/Session/Login");
            }

            var notification = await _context.Notifications.FindAsync(id);

            if (notification != null && notification.UserId == userId)
            {
                _context.Notifications.Remove(notification); 
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }



    }
}
