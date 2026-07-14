namespace ServianOps_Backend.Application.UserModule.User.UserDto
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public long[] RoleIds { get; set; }
        public long? TenantId { get; set; }
    }
}
