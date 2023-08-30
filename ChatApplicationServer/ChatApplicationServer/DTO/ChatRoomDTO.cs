using ChatApplicationServer.Models;

namespace ChatApplicationServer.DTO
{
    public class ChatRoomDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<Message> Messages { get; set; }
    }
}
