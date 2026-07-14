using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.UserModule.User.UserDto
{
    public class UserFilterDto : FilterDto
    {
        public long? TenantId { get; set; }
        public long? RoleId { get; set; }
    }
}
