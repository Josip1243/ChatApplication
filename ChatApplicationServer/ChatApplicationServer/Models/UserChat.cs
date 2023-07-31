using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServer.Models
{
    [Keyless]
    public class UserChat
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public bool Deleted { get; set; } = false;
    }
}
