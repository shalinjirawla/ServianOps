using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.RoleModule.Role.RoleDto;

namespace ServianOps_Backend.Application.RoleModule.Role
{
    public interface IRoleService
    {
        Task<StandardResponse<RoleDetailDto>> CreateRole(CreateRoleDto dto);
        Task<StandardResponse<RoleDetailDto>> UpdateRole(long id, UpdateRoleDto dto);
        Task<StandardResponse<RoleDetailDto>> GetRoleById(long id);
        Task<StandardResponse<PagedResultDto<RoleListDto>>> GetAllRoles(RoleFilterDto filter);
        Task<StandardResponse<IReadOnlyList<RoleLookupDto>>> GetRoleLookup();
        Task<StandardResponse<bool>> DeleteRole(long id);
    }
}
