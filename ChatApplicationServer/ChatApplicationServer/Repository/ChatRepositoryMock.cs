﻿using ChatApplicationServer.DTO;
using ChatApplicationServer.Models;

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

        List<ChatRoom> chatRooms = new List<ChatRoom>()
        {
            new ChatRoom() { Id = 1, Name = "AAA", Messages = messages },
            new ChatRoom() { Id = 2, Name = "BBB", Messages = messages2 },
        };



        public List<ChatRoom> GetAllChats(int userId)
        {
            var chatsInvolved = userChat.Where(x => x.UserId == userId).Select(x => x.ChatId);
            var chats = chatRooms.Where(x => chatsInvolved.Contains(x.Id));

            return chatRooms;
        }

        public ChatRoom GetChat(int chatId)
        {
            var chat = chatRooms.First(x => x.Id == chatId);

            return chat;
        }

        public ChatRoom AddChat(int user1, int user2)
        {
            var newChat = new ChatRoom()
            {
                Id = chatRooms.Count() + 1,
                CreatedAt = DateTime.Now,
            };

            userChat.Add(new UserChat()
            {
                ChatId = newChat.Id,
                UserId = user1,
            });
            userChat.Add(new UserChat()
            {
                ChatId = newChat.Id,
                UserId = user2,
            });

            chatRooms.Add(newChat);
            return newChat;
        } 
    }
}
