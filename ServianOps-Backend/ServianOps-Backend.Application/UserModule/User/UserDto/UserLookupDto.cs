namespace ServianOps_Backend.Application.UserModule.User.UserDto
{
    public class UserLookupDto
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string[] Roles { get; set; }
    }
}
