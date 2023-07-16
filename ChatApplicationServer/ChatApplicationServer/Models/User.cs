using System.Diagnostics;

namespace ChatApplicationServer.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public List<int> ActiveChats { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string RefreshToken { get; set; }
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
    }
}
