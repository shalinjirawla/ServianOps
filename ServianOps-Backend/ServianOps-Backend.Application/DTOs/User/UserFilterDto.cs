using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.User
{
    public class UserFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
        public long? TenantId { get; set; }
    }
}
