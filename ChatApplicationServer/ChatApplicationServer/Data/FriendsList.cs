using Microsoft.EntityFrameworkCore;

namespace ChatApplicationServer.Data
{
    [Keyless]
    public class FriendsList
    {
        public int UserId { get; set; }
        public int UserFriendId { get; set; }
    }
}
