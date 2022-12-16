using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.Hubs
{
    public class ChatHub : Hub
    {
        public async Task Test(string message)
        {
            await Clients.Clients(this.Context.ConnectionId).SendAsync(message);
        }
    }
}
