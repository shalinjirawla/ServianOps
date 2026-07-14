using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Role
{
    public class RoleFilterDto : PagedRequestDto
    {
        public bool? IsActive { get; set; }
    }
}
