namespace ChatApplicationServer.Models
{
    public class Connection
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string SignalRId { get; set; }
        public DateTime timeStamp { get; set; }
    }
}
