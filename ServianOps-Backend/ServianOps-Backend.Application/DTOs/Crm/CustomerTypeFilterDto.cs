using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class CustomerTypeFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
    }
}
