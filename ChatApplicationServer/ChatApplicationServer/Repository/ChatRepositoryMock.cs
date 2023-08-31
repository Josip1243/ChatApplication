using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Entity;
using System.Runtime.Intrinsics.X86;

namespace ChatApplicationServer.Repository
{
    public class ChatRepositoryMock
    {
        private ChatAppContext _appContext;

        public ChatRepositoryMock(ChatAppContext appContext)
        {
            _appContext = appContext;
        }

        public IEnumerable<ChatRoom> GetAllChats(int userId)
        {
            var chatsInvolved = _appContext.UsersChatRooms.Where(x => x.UserId == userId).Select(x => x.ChatRoomId);
            var chats = _appContext.ChatRooms.Where(x => chatsInvolved.Contains(x.Id));

            return chats;
        }

        public ChatRoomDTO GetChat(int chatId, string currentUsername)
        {
            var chat = _appContext.ChatRooms.First(x => x.Id == chatId);
            var allMessages = _appContext.Messages.Where(x => x.ChatId == chatId).ToList();
            return mapChatRoomToChatRoomDTO(chat, allMessages, currentUsername);
        }
        private ChatRoomDTO mapChatRoomToChatRoomDTO(ChatRoom chatRoom, List<Message> allMessages, string currentUsername)
        {
            var chatRoomDTO = new ChatRoomDTO()
            {
                Id = chatRoom.Id,
                Name = chatRoom.Name.Replace(currentUsername, "").Trim(),
                Messages = allMessages
            };

            return chatRoomDTO;
        }

        public IEnumerable<User> GetChatUsers(int chatId)
        {
            var userIds = _appContext.UsersChatRooms.Where(cr => cr.ChatRoomId == chatId).Select(uCR => uCR.UserId).ToList();
            var users = _appContext.Users.Where(user => userIds.Contains(user.Id)).ToList();

            return users;
        }

        public IEnumerable<UsersChatRoom> GetAllUsersChatRooms()
        {
            return _appContext.UsersChatRooms.ToList();
        }
        public IEnumerable<UsersChatRoom> GetUserChats(int userId)
        {
            var userChats = _appContext.UsersChatRooms.Where(uC => uC.UserId == userId);
            return userChats;
        }
        public IEnumerable<UsersChatRoom> GetUserChatsByChatId(int chatId)
        {
            var userChats = _appContext.UsersChatRooms.Where(uC => uC.ChatRoomId == chatId);
            return userChats;
        }

        public void UpdateUserChat(UsersChatRoom userChatRoom)
        {
            var temp = _appContext.UsersChatRooms.FirstOrDefault(uc => uc.UserId == userChatRoom.UserId && uc.ChatRoomId == userChatRoom.ChatRoomId);
            temp = userChatRoom;
            _appContext.SaveChanges();
        }

        public void AddMessage(MessageDTO messageDTO)
        {
            var newMessage = new Message()
            {
                ChatId = messageDTO.ChatId,
                Content = messageDTO.Content,
                SentAt = messageDTO.SentAt,
                UserId = messageDTO.UserId,
                Username = messageDTO.Username,
                InitializationVector = messageDTO.InitializationVector
            };
            _appContext.Messages.Add(newMessage);
            _appContext.SaveChanges();

            _appContext.MessagesChatRooms.Add(new MessagesChatRoom()
            {
                ChatRoomId = messageDTO.ChatId,
                MessageId = newMessage.Id,

            });
            _appContext.SaveChanges();
        }

        public List<Message> GetAllMessages(int chatRoomId)
        {
            return _appContext.Messages.Where(m => m.ChatId == chatRoomId).ToList();
        }

        public ChatRoom AddChat(User user1, User user2)
        {
            var newChat = new ChatRoom()
            {
                CreatedAt = DateTime.Now,
                Name = user2.Username + ' ' + user1.Username,
            };
            _appContext.ChatRooms.Add(newChat);
            _appContext.SaveChanges();
            var chatId = newChat.Id;

            _appContext.UsersChatRooms.Add(new UsersChatRoom()
            {
                ChatRoomId = chatId,
                UserId = user1.Id,
            });
            _appContext.UsersChatRooms.Add(new UsersChatRoom()      
            {
                ChatRoomId = chatId,
                UserId = user2.Id,
                Deleted = true
            });

            _appContext.SaveChanges();
            return newChat;
        } 

        public void AddUserChat(int chatRoomId, int userId)
        {
            if (!_appContext.UsersChatRooms.Any(uc => uc.ChatRoomId == chatRoomId && uc.UserId == userId))
            {
                _appContext.UsersChatRooms.Add(new UsersChatRoom()
                {
                    ChatRoomId = chatRoomId,
                    UserId = userId,
                    Deleted = false
                });
            }
            else
            {
                _appContext.UsersChatRooms.Remove(_appContext.UsersChatRooms.FirstOrDefault(uc => uc.UserId == userId && uc.ChatRoomId == chatRoomId));
                _appContext.UsersChatRooms.Add(new UsersChatRoom()
                {
                    ChatRoomId = chatRoomId,
                    UserId = userId,
                    Deleted = false
                });
            }
            _appContext.SaveChanges();
        }

        public void RemoveChat(int chatId, int userId)
        {
            var uC = _appContext.UsersChatRooms.First(usrCh => usrCh.ChatRoomId == chatId && usrCh.UserId == userId);

            if (uC != null)
                uC.Deleted = true;
            _appContext.SaveChanges();

            var userChats = _appContext.UsersChatRooms.Where(usrCh => usrCh.ChatRoomId == chatId).Select(uc => uc.Deleted).ToList();
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
                var userChatsToDelete = _appContext.UsersChatRooms.Where(uc => uc.ChatRoomId == chatId).ToList();
                var messageChatsToDelete = _appContext.MessagesChatRooms.Where(uc => uc.ChatRoomId == chatId).ToList();
                var chatRoomsToDelete = _appContext.ChatRooms.Where(uc => uc.Id == chatId).ToList();
                var messagesToDelete = _appContext.Messages.Where(uc => uc.ChatId == chatId).ToList();

                if (userChatsToDelete.Any())
                    _appContext.UsersChatRooms.RemoveRange(userChatsToDelete);

                if (messageChatsToDelete.Any())
                    _appContext.MessagesChatRooms.RemoveRange(messageChatsToDelete);

                if (messagesToDelete.Any())
                    _appContext.Messages.RemoveRange(messagesToDelete);

                if (chatRoomsToDelete.Any())
                    _appContext.ChatRooms.RemoveRange(chatRoomsToDelete);
            }
            _appContext.SaveChanges();
        }
    }
}
