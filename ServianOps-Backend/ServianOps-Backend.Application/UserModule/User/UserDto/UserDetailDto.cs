using System;
using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.UserModule.User.UserDto
{
    public class UserDetailDto : BaseAuditDto
    {
        public long? TenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsEmailVerified { get; set; }
        public string TenantName { get; set; }
        public string[] Roles { get; set; }
    }
}
