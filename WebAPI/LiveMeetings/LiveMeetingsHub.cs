using Microsoft.AspNetCore.SignalR;

namespace WebAPI.LiveMeetings
{
    /// <summary>
    /// SignalR hub for broadcasting live meeting events to connected clients.
    /// Clients receive updates via the "receiveMessage" method with <see cref="StorageEventDTO"/> payloads.
    /// Accessible at the "/live" endpoint.
    /// </summary>
    public class LiveMeetingsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
