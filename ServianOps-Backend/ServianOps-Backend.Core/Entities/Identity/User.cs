using System;
using ServianOps_Backend.Core.Entities.Base;
using ServianOps_Backend.Core.Entities.Saas;

namespace ServianOps_Backend.Core.Entities.Identity
{
    public class User : BaseEntity, IMayHaveTenant
    {
        public long? TenantId { get; set; }
        public Tenant Tenant { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        
        public string Phone { get; set; }
        public string ProfileImage { get; set; }
        
        public DateTime? LastLogin { get; set; }
        public bool IsEmailVerified { get; set; }
    }
}
