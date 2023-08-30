using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class Message
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public int ChatId { get; set; }

    public DateTime SentAt { get; set; }

    public string Content { get; set; } = null!;

    public virtual ChatRoom Chat { get; set; } = null!;

    public virtual ICollection<MessagesChatRoom> MessagesChatRooms { get; } = new List<MessagesChatRoom>();

    public virtual User User { get; set; } = null!;
}
