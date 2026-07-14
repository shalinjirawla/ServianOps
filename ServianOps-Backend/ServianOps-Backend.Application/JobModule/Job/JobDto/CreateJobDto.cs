using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Common.Enums;

namespace ServianOps_Backend.Application.JobModule.Job.JobDto
{
    public class CreateJobDto
    {
        public long CustomerId { get; set; }
        public long SiteId { get; set; }
        public long TradeId { get; set; }
        public string Description { get; set; }
        public PriorityEnum Priority { get; set; }
        public string PONumber { get; set; }
        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public IFormFileCollection Attachments { get; set; }
    }
}
