using ChatApplicationServer.DTO;
using ChatApplicationServer.HubConfig;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Optional.Collections;
using Optional.Unsafe;
using System.Security.Claims;
using Message = ChatApplicationServer.Models.Message;

namespace ChatApplicationServer.Services
{
    public class ChatService
    {
        private readonly ChatRepositoryMock _chatRepositoryMock;
        private readonly UserRepositoryMock _userRepositoryMock;
        private readonly ConnectionService _connectionService;

        public ChatService(ChatRepositoryMock chatRepositoryMock, UserRepositoryMock userRepositoryMock, ConnectionService connectionService)
        {
            _chatRepositoryMock = chatRepositoryMock;
            _userRepositoryMock = userRepositoryMock;
            _connectionService = connectionService;
        }

        public IEnumerable<ChatNameDTO> GetAllChats(string username)
        {
            var user = _userRepositoryMock.GetUser(username).ValueOrDefault();
            var chats = _chatRepositoryMock.GetAllChats(user.Id).ToList();
            var userChats = _chatRepositoryMock.GetUserChats(user.Id).ToList();

            return mapToChatNameDTO(user, chats, userChats);
        }

        public ChatRoomDTO GetChat(int chatId, string currentUsername)
        {
            var chat = _chatRepositoryMock.GetChat(chatId, currentUsername);
            if (chat != null)
                return chat;
            return null;
        }

        public IEnumerable<User> GetChatUsers(int chatId)
        {
            return _chatRepositoryMock.GetChatUsers(chatId);
        }

        public void AddMessage(MessageDTO messageDTO)
        {
            _chatRepositoryMock.AddMessage(messageDTO);
        }
        public ChatRoomDTO AddChat(string currentUser, string username)
        {
            var user1Optional = _userRepositoryMock.GetUser(currentUser);
            var user2Optional = _userRepositoryMock.GetUser(username);

            if (user1Optional.HasValue && user2Optional.HasValue)
            {
                var user1 = user1Optional.ValueOrFailure();
                var user2 = user2Optional.ValueOrFailure();

                if (user1.Id != user2.Id)
                {
                    var user1Chats = _chatRepositoryMock.GetAllChats(user1.Id).ToList();
                    var user2ChatIds = _chatRepositoryMock.GetAllChats(user2.Id).Select(u => u.Id).ToList();

                    var commonChat = user1Chats.FirstOrDefault(uC => user2ChatIds.Contains(uC.Id));

                    if (commonChat is not null)
                    {
                        var userChats = _chatRepositoryMock.GetUserChatsByChatId(commonChat.Id).ToList();
                        foreach (var userChat in userChats)
                        {
                            if (userChat.UserId == user1.Id)
                            {
                                userChat.Deleted = false;
                                _chatRepositoryMock.UpdateUserChat(userChat);
                            }
                        }
                        return mapToChatRoomDTO(commonChat, currentUser);
                    }

                    var newChat = _chatRepositoryMock.AddChat(user1, user2);

                    return mapToChatRoomDTO(newChat, currentUser);
                }
            }
            return null;
        }

        private ChatRoomDTO mapToChatRoomDTO(ChatRoom chatRoom, string currentUsername)
        {
            var chatName = chatRoom.Name.Replace(currentUsername, "").Trim();
            var messages = _chatRepositoryMock.GetAllMessages(chatRoom.Id).ToList();
            
            return new ChatRoomDTO() { Id = chatRoom.Id, Name = chatName, Messages = messages };
        }

        private List<MessageDTO> mapToMessageDTO(List<Message> messages)
        {
            List<MessageDTO> result = new List<MessageDTO>();

            if(messages is not null)
            {
                foreach (var message in messages)
                {

                    result.Add(new MessageDTO()
                    {
                        Id = message.Id,
                        ChatId = message.ChatId,
                        UserId = message.UserId,
                        Username = message.Username,
                        Content = message.Content,
                        SentAt = message.SentAt
                    });
                }
            }
            return result;
        }

        private IEnumerable<ChatNameDTO> mapToChatNameDTO(User user, IEnumerable<ChatRoom> chatRooms, IEnumerable<UsersChatRoom> userChats)
        {
            var chatDTOs = new List<ChatNameDTO>();

            if (chatRooms is not null)
            {
                foreach (var chatRoom in chatRooms)
                {
                    var userChat = userChats.FirstOrNone(uc => uc.ChatRoomId == chatRoom.Id && uc.UserId == user.Id).ValueOrDefault();
                    var otherUserId = _chatRepositoryMock.GetUserChatsByChatId(chatRoom.Id).Select(uc => uc.UserId).FirstOrDefault(uc => uc != user.Id);
                    var otherUser = _userRepositoryMock.GetUsers().ToList().FirstOrDefault(u => u.Id == otherUserId);
                    var chatName = otherUser.Username;

                    var onlineStatus = _connectionService.GetConnections().Any(c => c.UserId == otherUserId);

                    if (!userChat.Deleted)
                        chatDTOs.Add(new ChatNameDTO() { Id = chatRoom.Id, Name = chatName, Deleted = userChat.Deleted, 
                            UserInfo = new UserInfoDTO() { Id = otherUserId, Username = otherUser.Username, OnlineStatus = onlineStatus } });
                }
            }

            return chatDTOs;
        }
    }
}
