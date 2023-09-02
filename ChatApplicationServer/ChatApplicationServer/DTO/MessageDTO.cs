namespace ChatApplicationServer.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public byte[] InitializationVector { get; set; }
    }
}
