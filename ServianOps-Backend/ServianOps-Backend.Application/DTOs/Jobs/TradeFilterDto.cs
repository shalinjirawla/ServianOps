using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Jobs
{
    public class TradeFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
    }
}
