using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public byte[]? PasswordHash { get; set; }

    public byte[]? PasswordSalt { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? TokenCreated { get; set; }

    public DateTime? TokenExpires { get; set; }

    public virtual ICollection<Connection> Connections { get; } = new List<Connection>();

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<UsersChatRoom> UsersChatRooms { get; } = new List<UsersChatRoom>();
}
