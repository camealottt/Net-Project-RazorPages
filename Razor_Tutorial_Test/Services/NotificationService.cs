using Microsoft.AspNetCore.SignalR;
using Razor_Tutorial_Test.Data;
using Razor_Tutorial_Test.Model;
using Microsoft.EntityFrameworkCore;

namespace Razor_Tutorial_Test.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IHubContext<NotificationHub> hubContext,
            ApplicationDbContext context,
            ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _context = context;
            _logger = logger;
        }

        public async Task SendUnreadCountUpdateAsync(int userId)
        {
            try
            {
                // Get unread notification count
                int unreadCount = await _context.Notifications
                    .Where(n => n.UserId == userId && n.IsRead == false)
                    .CountAsync();

                _logger.LogInformation($"Sending unread count {unreadCount} to user {userId}");

                // Get all connections for this user
                var connections = NotificationHub.GetConnectionsForUser(userId.ToString());

                // If user has active connections, send them the update
                if (connections.Any())
                {
                    foreach (var connectionId in connections)
                    {
                        await _hubContext.Clients.Client(connectionId).SendAsync("UpdateUnreadCount", unreadCount);
                    }
                    _logger.LogInformation($"Sent to {connections.Count} connections for user {userId}");
                }
                else
                {
                    _logger.LogInformation($"No active connections found for user {userId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending notification to user {userId}");
            }
        }

        public async Task CreateNotificationAsync(int userId, string message, bool sendUpdate = true)
        {
            try
            {
                // Create and save notification
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                // Send update if requested
                if (sendUpdate)
                {
                    await SendUnreadCountUpdateAsync(userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating notification for user {userId}");
            }
        }
    }
}