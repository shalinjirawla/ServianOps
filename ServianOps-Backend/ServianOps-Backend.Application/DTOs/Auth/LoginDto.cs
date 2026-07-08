namespace ServianOps_Backend.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? TenancyName { get; set; }
    }
}
