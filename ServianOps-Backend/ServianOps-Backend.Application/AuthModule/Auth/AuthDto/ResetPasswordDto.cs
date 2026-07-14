namespace ServianOps_Backend.Application.AuthModule.Auth.AuthDto
{
    public class ResetPasswordDto
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
