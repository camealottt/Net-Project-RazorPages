namespace Razor_Tutorial_Test.Hubs
{
    using Microsoft.AspNetCore.SignalR;
    using Razor_Tutorial_Test.Data;
    using Razor_Tutorial_Test.Model;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;

        public ChatHub(ApplicationDbContext context)
        {
            _context = context;
        }

        // Store mapping of userId to connectionId
        private static ConcurrentDictionary<string, string> userConnections = new();

        public override Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                userConnections[userId.Value.ToString()] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            int senderIdInt = int.Parse(senderId);
            int receiverIdInt = int.Parse(receiverId);

            // Save to DB
            var chat = new ChatMessage
            {
                SenderId = senderIdInt,
                ReceiverId = receiverIdInt,
                Message = message,
                Timestamp = DateTime.Now
            };
            _context.ChatMessage.Add(chat);
            await _context.SaveChangesAsync();

            // Fetch sender details
            var sender = await _context.User.FindAsync(senderIdInt);
            var profilePic = string.IsNullOrEmpty(sender?.ProfilePicture)
                ? "/images/default-profile.png"
                : sender.ProfilePicture;

            var payload = new
            {
                SenderId = senderIdInt,
                Username = sender?.Username ?? "Unknown",
                ProfilePicUrl = profilePic,
                Message = message,
                Timestamp = chat.Timestamp.ToString("yyyy-MM-dd HH:mm:ss")
            };

            // Send to receiver
            if (userConnections.TryGetValue(receiverId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId)
                    .SendAsync("ReceiveMessage", payload);
            }

            // Send back to sender
            if (userConnections.TryGetValue(senderId, out var senderConnectionId))
            {
                await Clients.Client(senderConnectionId)
                    .SendAsync("ReceiveMessage", payload);
            }
        }

    }
}