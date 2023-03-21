using ChatApplicationServer.Models;
using Microsoft.AspNetCore.SignalR;
using ChatApplicationServer.Services;
using Optional.Unsafe;
using AutoMapper;
using ChatApplicationServer.DTO;
using Microsoft.Owin.Security.Provider;

namespace ChatApplicationServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private IUserService _userService;
        private IConnectionService _connectionService;
        private Mapper _mapper;

        public ChatHub(IUserService userService, IConnectionService connectionService) 
        { 
            _botUser = "Bot";
            _userService = userService;
            _connectionService = connectionService;
            _mapper = MapperConfig.InitializeAutoMapper();
        }

        //public async Task JoinRoom(UserConnection userConnection)
        //{
        //    await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);

        //    await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", _botUser, $"{userConnection.Name} has joined.");
        //}


        public async Task GetOnlineUsers()
        {
            var connections = _connectionService.GetConnections();
            var users = new List<UserDTO>();
            string currentSignalRId = Context.ConnectionId;

            foreach (var conn in connections)
            {
                if(currentSignalRId != conn.SignalRId)
                    users.Add(_userService.GetUserDTO(conn.PersonId).ValueOrDefault());
            }

            await Clients.Caller.SendAsync("getOnlineUsersResponse", users);
        }


        // Authorize user
        public async Task Authorize(UserCredentials userCredentials)
        {
            string currentSignalRId = Context.ConnectionId;
            var user = _userService.GetUser(userCredentials);

            if (user.HasValue)
            {
                Connection connection = new Connection()
                {
                    PersonId = user.ValueOrFailure().Id,
                    SignalRId = currentSignalRId,
                    timeStamp = DateTime.Now
                };
                _connectionService.AddConnection(connection);

                await Clients.Caller.SendAsync("authMeResponseSuccess", user);
                await Clients.Others.SendAsync("userOn", _mapper.Map<UserDTO>(user.ValueOrDefault()));
            }
            else
            {
                await Clients.Client(currentSignalRId).SendAsync("authMeResponseFail");
            }
        }
        // Log out user
        public async Task LogOut()
        {
            string currentSignalRId = Context.ConnectionId;
            var connection = _connectionService.GetConnection(currentSignalRId);

            _connectionService.RemoveConnection(currentSignalRId);

            var user = _userService.GetUserDTO(connection.Result.PersonId);

            await Clients.Others.SendAsync("userOff", user.ValueOrDefault());
        }
    }
}
 