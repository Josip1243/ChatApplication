using ChatApplicationServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly ConnectionService _connectionService;

        public ChatHub(ConnectionService connectionService) {
            _connectionService = connectionService;
        }

        public async Task askServer(int userId)
        {
            var signalrConnectionId = this.Context.ConnectionId;

            _connectionService.AddConnection(userId, signalrConnectionId);

            await Clients.Client(this.Context.ConnectionId).SendAsync("askServerListener", "Connection added!");
        }
    }
}
