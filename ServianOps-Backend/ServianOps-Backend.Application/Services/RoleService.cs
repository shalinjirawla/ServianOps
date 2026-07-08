using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Role;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.Services
{
    public class RoleService : Interfaces.IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<RoleDto> GetRoleByIdAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return null;

            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsActive = role.IsActive
            };
        }
    }
}
