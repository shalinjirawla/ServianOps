using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Role;

namespace ServianOps_Backend.Application.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDto> GetRoleByIdAsync(long id);
    }
}
