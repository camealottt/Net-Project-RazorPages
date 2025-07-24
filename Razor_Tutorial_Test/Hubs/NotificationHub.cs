using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class NotificationHub : Hub
{
    // Store userId -> connectionId
    private static ConcurrentDictionary<string, List<string>> _userConnections = new();
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger = null)
    {
        _logger = logger;
    }

    public override Task OnConnectedAsync()
    {
        try
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                string userIdString = userId.Value.ToString();

                // Add the connection to the user's connection list
                if (!_userConnections.ContainsKey(userIdString))
                {
                    _userConnections[userIdString] = new List<string>();
                }

                _userConnections[userIdString].Add(Context.ConnectionId);
                _logger?.LogInformation($"User {userIdString} connected with connection ID: {Context.ConnectionId}");
            }
            else
            {
                _logger?.LogWarning("User connected but no UserID found in session");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnConnectedAsync");
        }

        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        try
        {
            var httpContext = Context.GetHttpContext();
            var userId = httpContext?.Session.GetInt32("UserID");

            if (userId.HasValue)
            {
                string userIdString = userId.Value.ToString();

                if (_userConnections.TryGetValue(userIdString, out var connections))
                {
                    connections.Remove(Context.ConnectionId);

                    // If this was the last connection, remove the user entry
                    if (connections.Count == 0)
                    {
                        _userConnections.TryRemove(userIdString, out _);
                    }
                }

                _logger?.LogInformation($"User {userIdString} disconnected connection ID: {Context.ConnectionId}");
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error in OnDisconnectedAsync");
        }

        return base.OnDisconnectedAsync(exception);
    }

    // Add a static method for easier access from other classes
    public static List<string> GetConnectionsForUser(string userId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            return connections;
        }

        return new List<string>();
    }
}