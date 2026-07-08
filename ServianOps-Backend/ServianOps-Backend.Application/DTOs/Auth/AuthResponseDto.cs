namespace ServianOps_Backend.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public long UserId { get; set; }
        public long? TenantId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
