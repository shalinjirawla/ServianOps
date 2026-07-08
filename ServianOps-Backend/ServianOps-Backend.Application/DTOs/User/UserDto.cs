using System;

namespace ServianOps_Backend.Application.DTOs.User
{
    public class UserDto
    {
        public long Id { get; set; }
        public long? TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsActive { get; set; }
    }
}
