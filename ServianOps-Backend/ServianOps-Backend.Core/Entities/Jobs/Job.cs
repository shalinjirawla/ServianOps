using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Enums;

namespace ServianOps_Backend.Core.Entities.Jobs
{
    public class Job : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public string JobNumber { get; set; }

        public long CustomerId { get; set; }
        public Customer Customer { get; set; }

        public long SiteId { get; set; }
        public Site Site { get; set; }

        public long TradeId { get; set; }
        public Trade Trade { get; set; }

        public string Description { get; set; }

        public PriorityEnum Priority { get; set; }

        public string PONumber { get; set; }

        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public ICollection<JobAttachment> Attachments { get; set; }
    }
}
