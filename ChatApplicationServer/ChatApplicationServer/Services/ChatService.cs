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
            
            var user = _userRepositoryMock.GetUser(username);
            var chats = _chatRepositoryMock.GetAllChats(user.ValueOrDefault().Id);
            var userChats = _chatRepositoryMock.GetUserChats(user.ValueOrDefault().Id);

            return mapToChatNameDTO(chats, userChats, username);
        }

        public ChatDTO GetChat(int chatId, string currentUsername)
        {
            var chat = _chatRepositoryMock.GetChat(chatId);
            if (chat != null)
                return mapToChatDTO(chat, currentUsername);
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
        public ChatDTO AddChat(string currentUser, string username)
        {
            var user1Optional = _userRepositoryMock.GetUser(currentUser);
            var user2Optional = _userRepositoryMock.GetUser(username);

            if (user1Optional.HasValue && user2Optional.HasValue)
            {
                var user1 = user1Optional.ValueOrFailure();
                var user2 = user2Optional.ValueOrFailure();

                if (user1.Id != user2.Id)
                {
                    var user1Chats = _chatRepositoryMock.GetAllChats(user1.Id);
                    var user2Chats = _chatRepositoryMock.GetAllChats(user2.Id);

                    //var same = user1Chats.FirstOrDefault(uC => user2Chats.Select(u => u.Id).Contains(uC.Id));

                    var same = user1Chats.FirstOrDefault(uC => uC.Users.Any(u => u.Id == user2.Id));

                    if (same is not null)
                    {
                        var userChats = _chatRepositoryMock.GetUserChatsByChatId(same.Id);
                        foreach (var userChat in userChats)
                        {
                            if (userChat.UserId == user1.Id)
                            {
                                userChat.Deleted = false;
                                _chatRepositoryMock.UpdateUserChat(userChat);
                            }
                        }
                        return mapToChatDTO(same, currentUser);
                    }

                    var newChat = _chatRepositoryMock.AddChat(user1, user2);

                    return mapToChatDTO(newChat, currentUser);
                }
            }
            return null;
        }

        private ChatDTO mapToChatDTO(ChatRoom chatRoom, string currentUsername)
        {
            var chatName = chatRoom.Name.Replace(currentUsername, "").Trim();
            return new ChatDTO() { Id = chatRoom.Id, Name = chatName, Messages = mapToMessageDTO(chatRoom.Messages) };
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

        private IEnumerable<ChatNameDTO> mapToChatNameDTO(IEnumerable<ChatRoom> chatRooms, IEnumerable<UserChat> userChats, string currentUsername)
        {
            var chatDTOs = new List<ChatNameDTO>();

            if (chatRooms is not null)
            {
                foreach (var chatRoom in chatRooms)
                {
                    var userChat = userChats.FirstOrNone(uc => uc.ChatId == chatRoom.Id);
                    var chatName = chatRoom.Name.Replace(currentUsername, "").Trim();

                    chatDTOs.Add(new ChatNameDTO() { Id = chatRoom.Id, Name = chatName, Deleted = userChat.ValueOrDefault().Deleted });
                }
            }

            return chatDTOs;
        }
    }
}
