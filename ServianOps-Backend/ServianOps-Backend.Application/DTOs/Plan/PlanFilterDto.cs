using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Plan
{
    public class PlanFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
    }
}
