using Microsoft.AspNetCore.SignalR;

namespace WebAPI.LiveMeetings
{
    public class LiveMeetingsHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            return Task.CompletedTask;
        }
    }
}
