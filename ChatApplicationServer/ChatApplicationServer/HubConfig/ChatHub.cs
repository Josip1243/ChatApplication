using ChatApplicationServer.Services;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly ConnectionService _connectionService;
        private readonly ChatService _chatService;

        public ChatHub(ConnectionService connectionService, ChatService chatService) {
            _connectionService = connectionService;
            _chatService = chatService;
        }

        public async Task askServer(int userId)
        {
            var signalrConnectionId = this.Context.ConnectionId;

            _connectionService.AddConnection(userId, signalrConnectionId);

            await Clients.Client(this.Context.ConnectionId).SendAsync("askServerListener", "Connection added!");
        }

        public async Task sendMessage(string message, int chatRoomId)
        {
            _chatService.AddMessage(chatRoomId, message);
            var chatRoom = _chatService.GetChat(chatRoomId);
            var chatUsersIds = _chatService.GetChatUsers(chatRoomId);
            var connections = _connectionService.GetConnections(chatUsersIds).Select(con => con.SignalRId);

            await Clients.Clients(connections).SendAsync("receiveMessage", message);
        }
    }
}
