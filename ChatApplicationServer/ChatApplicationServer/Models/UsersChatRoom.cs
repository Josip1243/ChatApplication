using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class UsersChatRoom
{
    public int UserId { get; set; }

    public int ChatRoomId { get; set; }

    public bool Deleted { get; set; }

    public int Id { get; set; }

    public virtual ChatRoom ChatRoom { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
