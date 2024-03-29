﻿using ChatApplicationServer.DTO;
using ChatApplicationServer.HubConfig;
using ChatApplicationServer.Models;
using ChatApplicationServer.Repository;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Messaging;
using Optional.Collections;
using Optional.Unsafe;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApplicationServer.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConnectionService _connectionService;
        private readonly IConfiguration _configuration;

        public ChatService(IChatRepository chatRepository, IUserRepository userRepository, IConnectionService connectionService,
            IConfiguration configuration)
        {
            _chatRepository = chatRepository;
            _userRepository = userRepository;
            _connectionService = connectionService;
            _configuration = configuration;
        }

        public IEnumerable<ChatNameDTO> GetAllChats(string username)
        {
            var user = _userRepository.GetUser(username).ValueOrDefault();
            var chats = _chatRepository.GetAllChats(user.Id).ToList();
            var userChats = _chatRepository.GetUserChats(user.Id).ToList();

            return mapToChatNameDTO(user, chats, userChats);
        }

        public ChatRoomDTO GetChat(int chatId, string currentUsername)
        {
            var chat = _chatRepository.GetChat(chatId, currentUsername);
            if (chat != null)
                return mapToChatRoomDTO(chat, currentUsername);
            return null;
        }

        public IEnumerable<User> GetChatUsers(int chatId)
        {
            return _chatRepository.GetChatUsers(chatId);
        }

        public void AddMessage(MessageDTO messageDTO)
        {
            var keyString = _configuration.GetSection("Messages:SecretKey").Value;
            byte[] key = Encoding.UTF8.GetBytes(keyString);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.GenerateIV();
                messageDTO.InitializationVector = aesAlg.IV;

                // Encrypt the text and convert to base64-encoded string
                string encryptedBase64 = EncryptStringToBase64(messageDTO.Content, key, messageDTO.InitializationVector);
                messageDTO.Content = encryptedBase64;
            }

            _chatRepository.AddMessage(messageDTO);
        }

        public ChatRoomDTO AddChat(string currentUser, string username)
        {
            var user1Optional = _userRepository.GetUser(currentUser);
            var user2Optional = _userRepository.GetUser(username);

            if (user1Optional.HasValue && user2Optional.HasValue)
            {
                var user1 = user1Optional.ValueOrFailure();
                var user2 = user2Optional.ValueOrFailure();

                if (user1.Id != user2.Id)
                {
                    var user1Chats = _chatRepository.GetAllChats(user1.Id).ToList();
                    var user2ChatIds = _chatRepository.GetAllChats(user2.Id).Select(u => u.Id).ToList();

                    var commonChat = user1Chats.FirstOrDefault(uC => user2ChatIds.Contains(uC.Id));

                    if (commonChat is not null)
                    {
                        var userChats = _chatRepository.GetUserChatsByChatId(commonChat.Id).ToList();
                        foreach (var userChat in userChats)
                        {
                            if (userChat.UserId == user1.Id)
                            {
                                userChat.Deleted = false;
                                _chatRepository.UpdateUserChat(userChat);
                            }
                        }
                        return mapToChatRoomDTO(commonChat, currentUser);
                    }

                    var newChat = _chatRepository.AddChat(user1, user2);

                    return mapToChatRoomDTO(newChat, currentUser);
                }
            }
            return null;
        }

        private ChatRoomDTO mapToChatRoomDTO(ChatRoom chatRoom, string currentUsername)
        {
            var chatName = chatRoom.Name.Replace(currentUsername, "").Trim();
            var messages = _chatRepository.GetAllMessages(chatRoom.Id).ToList();
            var keyString = _configuration.GetSection("Messages:SecretKey").Value;
            byte[] key = Encoding.UTF8.GetBytes(keyString);

            foreach (var message in messages)
            {
                message.Content = DecryptBase64ToString(message.Content, key, message.InitializationVector);
            }

            return new ChatRoomDTO() { Id = chatRoom.Id, Name = chatName, Messages = messages };
        }

        private IEnumerable<ChatNameDTO> mapToChatNameDTO(User user, IEnumerable<ChatRoom> chatRooms, IEnumerable<UsersChatRoom> userChats)
        {
            var chatDTOs = new List<ChatNameDTO>();

            if (chatRooms is not null)
            {
                foreach (var chatRoom in chatRooms)
                {
                    var userChat = userChats.FirstOrNone(uc => uc.ChatRoomId == chatRoom.Id && uc.UserId == user.Id).ValueOrDefault();
                    var otherUserId = _chatRepository.GetUserChatsByChatId(chatRoom.Id).Select(uc => uc.UserId).FirstOrDefault(uc => uc != user.Id);
                    var otherUser = _userRepository.GetUsers().ToList().FirstOrDefault(u => u.Id == otherUserId);
                    var chatName = otherUser.Username;

                    var onlineStatus = _connectionService.GetConnections().Any(c => c.UserId == otherUserId);

                    if (!userChat.Deleted)
                        chatDTOs.Add(new ChatNameDTO()
                        {
                            Id = chatRoom.Id,
                            Name = chatName,
                            Deleted = userChat.Deleted,
                            UserInfo = new UserInfoDTO() { Id = otherUserId, Username = otherUser.Username, OnlineStatus = onlineStatus }
                        });
                }
            }

            return chatDTOs;
        }

        static string EncryptStringToBase64(string plainText, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        public static string DecryptBase64ToString(string encryptedBase64, byte[] key, byte[] iv)
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedBase64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
