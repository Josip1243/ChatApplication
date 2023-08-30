using Microsoft.AspNetCore.Mvc;

namespace ChatApplicationServer.Models2
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Expires { get; set; }
    }
}
