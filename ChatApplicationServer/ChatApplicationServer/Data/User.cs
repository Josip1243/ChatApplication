using System.Diagnostics;

namespace ChatApplicationServer.Data
{
    public class User 
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public Byte[]? Image { get; set; }
    }
}
