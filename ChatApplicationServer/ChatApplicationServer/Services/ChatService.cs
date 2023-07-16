using ChatApplicationServer.DTO;
using ChatApplicationServer.HubConfig;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Optional.Unsafe;
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

            return mapToChatNameDTO(chats);
        }

        public ChatDTO GetChat(int chatId)
        {
            return mapToChatDTO(_chatRepositoryMock.GetChat(chatId));
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

                    var same = user1Chats.FirstOrDefault(uC => user2Chats.Select(u => u.Id).Contains(uC.Id));

                    if (same is not null)
                    {
                        return mapToChatDTO(same);
                    }

                    var newChat = _chatRepositoryMock.AddChat(user1, user2);

                    return mapToChatDTO(newChat);
                }
            }
            return null;
        }

        private ChatDTO mapToChatDTO(ChatRoom chatRoom)
        {
            return new ChatDTO() { Id = chatRoom.Id, Name = chatRoom.Name, Messages = mapToMessageDTO(chatRoom.Messages) };
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

        private IEnumerable<ChatNameDTO> mapToChatNameDTO(IEnumerable<ChatRoom> chatRooms)
        {
            var chatDTOs = new List<ChatNameDTO>();

            if (chatRooms is not null)
            {
                foreach (var chatRoom in chatRooms)
                {
                    chatDTOs.Add(new ChatNameDTO() { Id = chatRoom.Id, Name = chatRoom.Name });
                }
            }

            return chatDTOs;
        }
    }
}
