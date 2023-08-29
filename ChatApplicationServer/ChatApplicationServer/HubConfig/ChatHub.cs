using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using ChatApplicationServer.Services;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using Optional.Unsafe;
using System.Security.Claims;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly ConnectionService _connectionService;
        private readonly ChatService _chatService;
        private readonly ChatRepositoryMock _chatRepositoryMock;
        private readonly IUserService _userService;

        public ChatHub(ConnectionService connectionService, ChatService chatService, ChatRepositoryMock chatRepositoryMock, IUserService userService)
        {
            _connectionService = connectionService;
            _chatService = chatService;
            _chatRepositoryMock = chatRepositoryMock;
            _userService = userService;
        }

        public async Task askServer(int userId)
        {
            var signalrConnectionId = this.Context.ConnectionId;

            _connectionService.AddConnection(userId, signalrConnectionId);

            await Clients.Client(this.Context.ConnectionId).SendAsync("askServerListener", "Connection added!");
        }


        // Implement trigger for adding chat
        public async Task addChat(string currentUsername, string targetedUsername)
        {
            var currentUser = _userService.GetUser(currentUsername).ValueOrDefault();
            var targetedUser = _userService.GetUser(targetedUsername).ValueOrDefault();

            var userIds = new List<int>();
            userIds.Add(currentUser.Id);
            userIds.Add(targetedUser.Id);

            _chatService.AddChat(currentUsername, targetedUsername);

            await Clients.Client(this.Context.ConnectionId).SendAsync("addChatListener");
        }


        public async Task disconnect()
        {
            var signalrConnectionId = this.Context.ConnectionId;
            _connectionService.RemoveConnection(signalrConnectionId);
            await Clients.Client(this.Context.ConnectionId).SendAsync("disconnect", "Connection removed!");
        }

        public async Task sendMessage(MessageDTO messageDTO)
        {
            _chatService.AddMessage(messageDTO);
            //var chatRoom = _chatService.GetChat(messageDTO.ChatId);
            var chatUsers = _chatService.GetChatUsers(messageDTO.ChatId);

            foreach (var user in chatUsers)
            {
                _chatRepositoryMock.AddUserChat(messageDTO.ChatId, user.Id);
            }
            var connections = _connectionService.GetConnections(chatUsers.Select(u => u.Id)).Select(con => con.SignalRid);

            await Clients.Clients(connections).SendAsync("receiveMessage", messageDTO);
        }
    }
}
