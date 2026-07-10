using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Jobs
{
    public class Trade : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public string Name { get; set; }

        public ICollection<Job> Jobs { get; set; }
    }
}
