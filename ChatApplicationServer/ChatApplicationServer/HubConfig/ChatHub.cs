using Microsoft.AspNetCore.SignalR;

namespace ChatApplicationServer.HubConfig
{
    public class ChatHub : Hub
    {
        public async Task askServer(string someText)
        {
            string tempString = "message was: " + someText;
        }
    }
}
