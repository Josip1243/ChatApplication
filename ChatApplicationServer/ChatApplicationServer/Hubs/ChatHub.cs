using ChatApplicationServer.Models;
using Microsoft.AspNetCore.SignalR;
using ChatApplicationServer.Services;
using Optional.Unsafe;

namespace ChatApplicationServer.Hubs
{
    public class ChatHub : Hub
    {
        private readonly string _botUser;
        private IUserService _userService;
        private IConnectionService _connectionService;

        public ChatHub(IUserService userService, IConnectionService connectionService) 
        { 
            _botUser = "Bot";
            _userService = userService;
            _connectionService = connectionService;
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);

            await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", _botUser, $"{userConnection.Name} has joined.");
        }

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
            }
            else
            {
                await Clients.Client(currentSignalRId).SendAsync("authMeResponseFail");
            }
        }

        public async Task LogOut()
        {
            string currentSignalRId = Context.ConnectionId;

            _connectionService.RemoveConnection(currentSignalRId);
        }
    }
}
 