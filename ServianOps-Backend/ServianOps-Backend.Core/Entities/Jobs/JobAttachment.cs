using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Jobs
{
    public class JobAttachment : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public long JobId { get; set; }
        public Job Job { get; set; }

        public string Link { get; set; }
    }
}
