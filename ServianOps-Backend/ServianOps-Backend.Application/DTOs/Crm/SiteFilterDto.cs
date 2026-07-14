using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class SiteFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
        public long? CustomerId { get; set; }
    }
}
