using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class MessagesChatRoom
{
    public int MessageId { get; set; }

    public int ChatRoomId { get; set; }

    public int Id { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual Message Message { get; set; } = null!;
}
