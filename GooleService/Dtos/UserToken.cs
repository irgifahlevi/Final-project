namespace Goole_OpenId.Dtos
{
    public class UserToken
    {
        public string? Token { get; set; }
        public string? ExpiredAt { get; set; }
        public string Message { get; set; }
        public string? Username { get; set; }
    }
}
