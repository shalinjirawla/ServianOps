using System;

namespace ServianOps_Backend.Application.DTOs.User
{
    public class CreateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public long[] RoleIds { get; set; }
    }
}
