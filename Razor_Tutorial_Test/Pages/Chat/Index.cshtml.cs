using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using System.Collections.Generic;
using System.Linq;

namespace Razor_Tutorial_Test.Pages.Chat
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string ReceiverUsername { get; set; }
        public List<User> AllUsers { get; set; }
        public List<ChatDisplayModel> DisplayMessages { get; set; } = new();
        public string ReceiverProfilePic { get; set; }


        public class ChatDisplayModel
        {
            public int SenderId { get; set; }
            public string Username { get; set; }
            public string ProfilePicUrl { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }
            public string FormattedTime => Timestamp.ToString("hh:mm tt").ToLower();

        }
        public void OnGet(int? receiverId)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
            {
                Response.Redirect("/Session/Login");
                return;
            }

            SenderId = userId.Value;
           
            if (receiverId.HasValue)
            {
                ReceiverId = receiverId.Value;

                var receiver = _context.User.FirstOrDefault(u => u.Id == ReceiverId);
                ReceiverUsername = receiver?.Username ?? "Unknown";
                ReceiverProfilePic = string.IsNullOrEmpty(receiver?.ProfilePicture)
                    ? "/images/default-profile.png"
                    : receiver.ProfilePicture;

                // Load raw messages
                var messages = _context.ChatMessage
                    .Where(m =>
                        (m.SenderId == SenderId && m.ReceiverId == ReceiverId) ||
                        (m.SenderId == ReceiverId && m.ReceiverId == SenderId))
                    .OrderBy(m => m.Timestamp)
                    .ToList();

                // Fetch both users' info
                var userMap = _context.User
                    .Where(u => u.Id == SenderId || u.Id == ReceiverId)
                    .ToDictionary(u => u.Id, u => u);

                // Transform into display model
                DisplayMessages = messages.Select(m => new ChatDisplayModel
                {
                    SenderId = m.SenderId,
                    Username = userMap[m.SenderId].Username,
                    ProfilePicUrl = string.IsNullOrEmpty(userMap[m.SenderId].ProfilePicture)
                    ? "/images/default-profile.png"
                    : userMap[m.SenderId].ProfilePicture,
                    Message = m.Message,
                    Timestamp = m.Timestamp
                }).ToList();

            }
            else
            {
                ReceiverUsername = "No one selected";
                DisplayMessages = new List<ChatDisplayModel>();
            }
        }
    }
}
