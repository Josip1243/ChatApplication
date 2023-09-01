using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;

namespace ChatApplicationServer.Repository
{
    public interface IChatRepository
    {
        ChatRoom AddChat(User user1, User user2);
        void AddMessage(MessageDTO messageDTO);
        void AddUserChat(int chatRoomId, int userId);
        IEnumerable<ChatRoom> GetAllChats(int userId);
        List<Message> GetAllMessages(int chatRoomId);
        IEnumerable<UsersChatRoom> GetAllUsersChatRooms();
        ChatRoom GetChat(int chatId, string currentUsername);
        IEnumerable<User> GetChatUsers(int chatId);
        IEnumerable<UsersChatRoom> GetUserChats(int userId);
        IEnumerable<UsersChatRoom> GetUserChatsByChatId(int chatId);
        void RemoveChat(int chatId, int userId);
        void UpdateUserChat(UsersChatRoom userChatRoom);
    }
}