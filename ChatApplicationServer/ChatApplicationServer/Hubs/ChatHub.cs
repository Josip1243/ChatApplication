using ChatApplicationServer.Data;
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

        public async Task Test(UserConnection inputUser)
        {
            using (var ctx = context)
            {

                User user = ctx.Users.First(ctx=> ctx.Id == inputUser.Id);

                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            string response = "Added new user!";
            await Clients.Clients(this.Context.ConnectionId).SendAsync("serverResponse", response);
        }
    }
}
 