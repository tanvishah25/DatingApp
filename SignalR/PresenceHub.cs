using DatingApp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker) { 
            _presenceTracker = presenceTracker;
        }
        public override async Task OnConnectedAsync()
        {
            //When user loggedin add username and connectionid inside dictionary created in presencetracker
            if (Context.User == null) throw new HubException("Cannot get current user claim");

            await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);

            // When user connects to the hub  "UserIsOnline" this will get invoked
            await Clients.Others.SendAsync("UserIsOnline",Context.User?.GetUserName());
            // Get all online users
            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User == null) throw new HubException("Cannot get current user claim");
            await _presenceTracker.UserDisConnected(Context.User.GetUserName(), Context.ConnectionId);
            // Get all online users
            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
            await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
