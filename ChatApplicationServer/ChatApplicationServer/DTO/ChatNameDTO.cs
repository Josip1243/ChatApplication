namespace ChatApplicationServer.DTO
{
    public class ChatNameDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public UserInfoDTO UserInfo { get; set; }
    }
}
