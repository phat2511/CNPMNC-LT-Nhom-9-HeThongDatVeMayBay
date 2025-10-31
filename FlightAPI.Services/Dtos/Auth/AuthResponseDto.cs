namespace FlightAPI.Services.Dtos.Auth // <-- Namespace đúng
{
    public class AuthResponseDto // <-- Class là PUBLIC
    {
        public string Token { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public List<string> Roles { get; set; }
    }
}