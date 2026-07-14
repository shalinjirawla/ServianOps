namespace ServianOps_Backend.Application.AuthModule.Auth.AuthDto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? TenancyName { get; set; }
    }
}
