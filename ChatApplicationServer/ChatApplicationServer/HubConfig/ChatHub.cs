﻿using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using ChatApplicationServer.Services;
using Microsoft.AspNet.SignalR.Messaging;
using Microsoft.AspNetCore.SignalR;
using Optional.Unsafe;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        private readonly IConnectionService _connectionService;
        private readonly IChatService _chatService;
        private readonly IChatRepository _chatRepository;
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public ChatHub(IConnectionService connectionService, IChatService chatService, IChatRepository chatRepository, IUserService userService, IConfiguration configuration)
        {
            _connectionService = connectionService;
            _chatService = chatService;
            _chatRepository = chatRepository;
            _userService = userService;
            _configuration = configuration;
        }

        // Method for establishing connection
        public async Task connect(int userId)
        {
            var signalrConnectionId = this.Context.ConnectionId;
            var connections = _connectionService.GetConnections();
            var connection = _connectionService.GetConnections().FirstOrDefault(c => c.SignalRid == signalrConnectionId);
            var chatRoomIds = _chatRepository.GetUserChats(userId).Select(uc => uc.ChatRoomId).ToList();
            List<string> connectionsToSendTo = new List<string>();

            foreach (var chatRoomId in chatRoomIds)
            {
                var user = _chatRepository.GetChatUsers(chatRoomId).FirstOrDefault(u => u.Id != userId);
                var tempConnection = connections.FirstOrDefault(c => c.UserId == user.Id);

                if (tempConnection != null)
                {
                    var tempConnectionId = tempConnection.SignalRid;
                    connectionsToSendTo.Add(tempConnectionId);
                }
            }

            var existingConnections = connections.Where(c => c.UserId == userId).ToList();
            foreach (var existingConnection in existingConnections)
            {
                _connectionService.RemoveConnection(existingConnection.SignalRid);
            }
            _connectionService.AddConnection(userId, signalrConnectionId);

            await Clients.Client(this.Context.ConnectionId).SendAsync("connectListener", "Connection added!");
            await Clients.Clients(connectionsToSendTo).SendAsync("onlineStatusChange");
        }
        public async override Task OnConnectedAsync()
        {
            var signalrConnectionId = this.Context.ConnectionId;

            await base.OnConnectedAsync();
        }


        // Triggers on adding chat
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

        // Disconnecting logic and also onlineStatus refresher
        public async Task disconnect()
        {
            await OnDisconnectedAsync(null);
        }
        public async override Task OnDisconnectedAsync(Exception? exception)
        {
            var signalrConnectionId = this.Context.ConnectionId;
            var connections = _connectionService.GetConnections();
            var connection = _connectionService.GetConnections().FirstOrDefault(c => c.SignalRid == signalrConnectionId);
            var userId = connection.UserId;

            var chatRoomIds = _chatRepository.GetUserChats(userId).Select(uc => uc.ChatRoomId).ToList();

            List<string> connectionsToSendTo = new List<string>();

            foreach (var chatRoomId in chatRoomIds)
            {
                var user = _chatRepository.GetChatUsers(chatRoomId).FirstOrDefault(u => u.Id != userId);
                var tempConnection = connections.FirstOrDefault(c => c.UserId == user.Id);

                if (tempConnection != null)
                {
                    var tempConnectionId = tempConnection.SignalRid;
                    connectionsToSendTo.Add(tempConnectionId);
                }
            }
            _connectionService.RemoveConnection(signalrConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("disconnect", "Connection removed!");
            await Clients.Clients(connectionsToSendTo).SendAsync("onlineStatusChange");

            await base.OnDisconnectedAsync(exception);
        }

        public async Task sendMessage(MessageDTO messageDTO)
        {
            _chatService.AddMessage(messageDTO);
            var chatUsers = _chatService.GetChatUsers(messageDTO.ChatId);

            foreach (var user in chatUsers)
            {
                _chatRepository.AddUserChat(messageDTO.ChatId, user.Id);
            }
            var connections = _connectionService.GetConnections(chatUsers.Select(u => u.Id)).Select(con => con.SignalRid);

            var keyString = _configuration.GetSection("Messages:SecretKey").Value;
            byte[] key = Encoding.UTF8.GetBytes(keyString);

            messageDTO.Content = ChatService.DecryptBase64ToString(messageDTO.Content, key, messageDTO.InitializationVector);
            await Clients.Clients(connections).SendAsync("receiveMessage", messageDTO);
        }
    }
}
