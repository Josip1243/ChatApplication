using System;
using System.Collections.Generic;

namespace ChatApplicationServer.Models2;

public partial class Connection
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string SignalRid { get; set; } = null!;

    public DateTime TimeStamp { get; set; }

    public virtual User User { get; set; } = null!;
}
