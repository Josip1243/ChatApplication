using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class ChatRoom
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Message> Messages { get; } = new List<Message>();

    public virtual ICollection<MessagesChatRoom> MessagesChatRooms { get; } = new List<MessagesChatRoom>();

    public virtual ICollection<UsersChatRoom> UsersChatRooms { get; } = new List<UsersChatRoom>();
}
