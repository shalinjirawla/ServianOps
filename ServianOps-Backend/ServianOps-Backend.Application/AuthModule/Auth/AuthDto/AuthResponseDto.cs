namespace ServianOps_Backend.Application.AuthModule.Auth.AuthDto
{
    public class AuthResponseDto
    {
        public long UserId { get; set; }
        public long? TenantId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
