namespace ChatApplicationServer.Data
{
    public class ChatRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DeleteAfter { get; set; }
        public DateTime LastActivity { get; set; }
        public bool IsGroup { get; set; }
        public bool ShouldDelete { get; set; }
    }
}
