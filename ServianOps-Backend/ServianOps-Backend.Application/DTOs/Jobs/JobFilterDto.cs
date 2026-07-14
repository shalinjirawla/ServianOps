using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Jobs
{
    public class JobFilterDto : PagedRequestDto
    {
        public long? CustomerId { get; set; }
        public long? SiteId { get; set; }
        public string? Status { get; set; }
    }
}
