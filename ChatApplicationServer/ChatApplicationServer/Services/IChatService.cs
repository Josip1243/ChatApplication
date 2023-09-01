using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;

namespace ChatApplicationServer.Services
{
    public interface IChatService
    {
        ChatRoomDTO AddChat(string currentUser, string username);
        void AddMessage(MessageDTO messageDTO);
        IEnumerable<ChatNameDTO> GetAllChats(string username);
        ChatRoomDTO GetChat(int chatId, string currentUsername);
        IEnumerable<User> GetChatUsers(int chatId);
    }
}