using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using ChatApplicationServer.Services;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly ConnectionService _connectionService;
        private readonly ChatService _chatService;
        private readonly ChatRepositoryMock _chatRepositoryMock;

        public ChatHub(ConnectionService connectionService, ChatService chatService, ChatRepositoryMock chatRepositoryMock) {
            _connectionService = connectionService;
            _chatService = chatService;
            _chatRepositoryMock = chatRepositoryMock;
        }

        public async Task askServer(int userId)
        {
            var signalrConnectionId = this.Context.ConnectionId;

            _connectionService.AddConnection(userId, signalrConnectionId);

            await Clients.Client(this.Context.ConnectionId).SendAsync("askServerListener", "Connection added!");
        }

        public async Task sendMessage(MessageDTO messageDTO)
        {
            _chatService.AddMessage(messageDTO);
            var chatRoom = _chatService.GetChat(messageDTO.ChatId);
            var chatUsers = _chatService.GetChatUsers(messageDTO.ChatId);

            foreach (var user in chatUsers) 
            {
                _chatRepositoryMock.AddUserChat(messageDTO.ChatId, user.Id);
            }
            var connections = _connectionService.GetConnections(chatUsers.Select(u => u.Id)).Select(con => con.SignalRId);

            await Clients.Clients(connections).SendAsync("receiveMessage", messageDTO);
        }
    }
}
