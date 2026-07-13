using System;
using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Identity
{
    public class Role : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
