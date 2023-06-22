using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNet.SignalR.Messaging;
using Optional.Unsafe;
using Message = ChatApplicationServer.Models.Message;

namespace ChatApplicationServer.Services
{
    public class ChatService
    {
        private readonly ChatRepositoryMock _chatRepositoryMock;
        private readonly UserRepositoryMock _userRepositoryMock;

        public ChatService(ChatRepositoryMock chatRepositoryMock, UserRepositoryMock userRepositoryMock)
        {
            _chatRepositoryMock = chatRepositoryMock;
            _userRepositoryMock = userRepositoryMock;
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

        public IEnumerable<int> GetChatUsers(int chatId)
        {
            return _chatRepositoryMock.GetChatUsers(chatId);
        }

        public void AddMessage(int chatRoomId, string message)
        {
            _chatRepositoryMock.AddMessage(chatRoomId, message);
        }
        public ChatDTO AddChat(string currentUser, string username)
        {
            var user1 = _userRepositoryMock.GetUser(currentUser);
            var user2 = _userRepositoryMock.GetUser(username);

            if (user1.HasValue && user2.HasValue)
            {
                if (user1.ValueOrDefault() != user2.ValueOrDefault())
                {
                    var user1Chats = _chatRepositoryMock.GetAllChats(user1.ValueOrDefault().Id);
                    var user2Chats = _chatRepositoryMock.GetAllChats(user2.ValueOrDefault().Id);

                    var same = user1Chats.FirstOrDefault(uC => user2Chats.Select(u => u.Id).Contains(uC.Id));

                    if (same is not null) {
                        return mapToChatDTO(same);
                    }

                    var newChat = _chatRepositoryMock.AddChat(user1.ValueOrDefault(), user2.ValueOrDefault());
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
                        Content = message.Content
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
