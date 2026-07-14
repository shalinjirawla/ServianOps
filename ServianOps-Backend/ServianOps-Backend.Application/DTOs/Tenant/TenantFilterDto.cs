using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Tenant
{
    public class TenantFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
    }
}
