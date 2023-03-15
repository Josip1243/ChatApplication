﻿namespace ChatApplicationServer.Models
{
    public class UserConnection
    {
        public int? Id { get; set; } = null;
        public string Name { get; set; }
        public string ConnectionId { get; set; }
        public string ChatRoom { get; set; }
    }
}
