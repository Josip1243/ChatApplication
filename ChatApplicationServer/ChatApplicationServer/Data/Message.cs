﻿namespace ChatApplicationServer.Data
{
    public class Message
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public DateTime SentAt { get; set; }
        public string Content { get; set; }
        public byte[] Image { get; set; }
    }
}
