using System;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Identity
{
    public class UserRole : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
        
        public long RoleId { get; set; }
        public Role Role { get; set; }
    }
}
