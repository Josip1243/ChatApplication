using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServer.Data
{
    [Keyless]
    public class UserChat
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
