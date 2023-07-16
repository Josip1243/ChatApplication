using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using System.Runtime.Intrinsics.X86;

namespace ChatApplicationServer.Repository
{
    public class ChatRepositoryMock
    {
        List<UserChat> userChat = new List<UserChat>()
        {
            new UserChat() { UserId = 1, ChatId = 1},
            new UserChat() { UserId = 1, ChatId = 2},
        };

        static List<Message> messages = new List<Message>()
        {
            new Message() {Id = 1, ChatId = 1, Content = "Aaaaa", UserId = 1, Username = "Pero"},
            new Message() {Id = 2, ChatId = 1, Content = "Be", UserId = 1, Username = "Pero"},
            new Message() {Id = 3, ChatId = 1, Content = "Ce", UserId = 1, Username = "Pero"},
            new Message() {Id = 4, ChatId = 1, Content = "De", UserId = 1, Username = "Pero"},
        };
        static List<Message> messages2 = new List<Message>()
        {
            new Message() {Id = 1, ChatId = 2, Content = "Aaaaa", UserId = 1, Username = "Pero"},
            new Message() {Id = 2, ChatId = 2, Content = "Be", UserId = 1, Username = "Pero"},
        };

        List<ChatRoom> chatRooms = new List<ChatRoom>();



        public IEnumerable<ChatRoom> GetAllChats(int userId)
        {
            var chatsInvolved = userChat.Where(x => x.UserId == userId).Select(x => x.ChatId);
            var chats = chatRooms.Where(x => chatsInvolved.Contains(x.Id));

            return chats;
        }

        public ChatRoom GetChat(int chatId)
        {
            var chat = chatRooms.First(x => x.Id == chatId);
            chat.Messages = messages.FindAll(x => x.ChatId == chatId);
            return chat;
        }

        public IEnumerable<User> GetChatUsers(int chatId)
        {
            var chatRoom = chatRooms.First(cr => cr.Id == chatId);
            return chatRoom.Users;
        }

        public void AddMessage(MessageDTO messageDTO)
        {
            messages.Add(new Message()
            {
                ChatId = messageDTO.ChatId,
                Content = messageDTO.Content,
                SentAt = messageDTO.SentAt,
                UserId = messageDTO.UserId,
                Username = messageDTO.Username,
            });
        }

        public ChatRoom AddChat(User user1, User user2)
        {
            var newChat = new ChatRoom()
            {
                Id = chatRooms.Count() + 1,
                CreatedAt = DateTime.Now,
                Name = user2.Username + ' ' + user1.Username,
                Users = new List<User>()
            };
            newChat.Users.Add(user1);
            newChat.Users.Add(user2);

            userChat.Add(new UserChat()
            {
                ChatId = newChat.Id,
                UserId = user1.Id,
            });

            chatRooms.Add(newChat);
            return newChat;
        } 

        public void AddUserChat(int chatRoomId, int userId)
        {
            if (!userChat.Any(uc => uc.ChatId == chatRoomId && uc.UserId == userId))
            {
                userChat.Add(new UserChat()
                {
                    ChatId = chatRoomId,
                    UserId = userId
                });
            }
        }

        public void RemoveChat(int chatId, int userId)
        {
            var uC = userChat.First(usrCh => usrCh.ChatId == chatId && usrCh.UserId == userId);

            if(uC != null) 
                userChat.Remove(uC);
        }
    }
}
