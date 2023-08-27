using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

namespace ChatApplicationServer.Repository
{
    public class ChatRepositoryMock
    {
        List<UserChat> userChat = new List<UserChat>();
        static List<Message> messages = new List<Message>();
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

        public IEnumerable<UserChat> GetUserChats(int userId)
        {
            var userChats = userChat.FindAll(uC => uC.UserId == userId);
            return userChats;
        }
        public IEnumerable<UserChat> GetUserChatsByChatId(int chatId)
        {
            var userChats = userChat.FindAll(uC => uC.ChatId == chatId);
            return userChats;
        }

        public void UpdateUserChat(UserChat uC)
        {
            userChat.Remove(userChat.Find(uc => uc.UserId == uC.UserId && uc.ChatId == uC.ChatId));
            userChat.Add(uC);
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
                    UserId = userId,
                    Deleted = false
                });
            }
            else
            {
                userChat.Remove(userChat.Find(uc => uc.UserId == userId && uc.ChatId == chatRoomId));
                userChat.Add(new UserChat()
                {
                    ChatId = chatRoomId,
                    UserId = userId,
                    Deleted = false
                });
            }
        }

        public void RemoveChat(int chatId, int userId)
        {
            var uC = userChat.First(usrCh => usrCh.ChatId == chatId && usrCh.UserId == userId);

            if (uC != null)
                uC.Deleted = true;

            var userChats = userChat.FindAll(usrCh => usrCh.ChatId == chatId).Select(uc => uc.Deleted);
            var delete = true;
            foreach(var deleted in userChats)
            {
                if (!deleted)
                {
                    delete = false;
                    break;
                }
            }

            if (delete)
            {
                userChat.RemoveAll(uc => uc.ChatId == chatId);
                chatRooms.Remove(chatRooms.Find(cR => cR.Id == chatId));
                messages.RemoveAll(m => m.ChatId == chatId);
            }
        }
    }
}
