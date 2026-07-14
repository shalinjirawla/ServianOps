using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Role;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDto> GetRoleByIdAsync(long id);
        Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<RoleDto>> GetRolesPagedAsync(RoleFilterDto filter);
    }
}
