namespace ChatApplicationServer.Data
{
    public class Connection
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string SignalRId { get; set; }
        public DateTime timeStamp { get; set; }
    }
}
