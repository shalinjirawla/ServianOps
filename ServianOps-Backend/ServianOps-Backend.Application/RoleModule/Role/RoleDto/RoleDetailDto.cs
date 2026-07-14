using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.RoleModule.Role.RoleDto
{
    public class RoleDetailDto : BaseAuditDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
