using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.JobModule.Job.JobDto
{
    public class JobFilterDto : FilterDto
    {
        public long? CustomerId { get; set; }
        public long? SiteId { get; set; }
        public string Status { get; set; }
    }
}
