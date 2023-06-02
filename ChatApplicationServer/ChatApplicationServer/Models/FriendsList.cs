using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServer.Models
{
    [Keyless]
    public class FriendsList
    {
        public int UserId { get; set; }
        public int UserFriendId { get; set; }
    }
}
