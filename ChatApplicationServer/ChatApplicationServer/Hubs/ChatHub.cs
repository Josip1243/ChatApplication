using ChatApplicationServer.Data;
using ChatApplicationServer.HubModels;
using ChatApplicationServer.Models;
using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.Hubs
{
    public class ChatHub : Hub
    {
        ChatContext context;
        private readonly string _botUser;

        public ChatHub(ChatContext options) 
        { 
            this.context = options;
            _botUser = "Bot";
        }

        public async Task JoinRoom(UserConnection userConnection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);

            await Clients.Group(userConnection.ChatRoom).SendAsync("ReceiveMessage", _botUser, $"{userConnection.Name} has joined.");
        }


        public async Task Authorize(UserInfo userInfo)
        {
            string currentSignalRId = Context.ConnectionId;
            User? user = context.Users.Where(u => u.Username == userInfo.Username && u.Password == userInfo.Password).SingleOrDefault();

            if (user != null)
            {
                Connection connection = new Connection()
                {
                    PersonId = user.Id,
                    SignalRId = currentSignalRId,
                    timeStamp = DateTime.Now
                };

                await context.Connections.AddAsync(connection);
                context.SaveChanges();

                await Clients.Caller.SendAsync("authMeResponseSuccess", user);
            }
            else
            {
                await Clients.Client(currentSignalRId).SendAsync("authMeResponseFail");
            }
        }


    }
}
 