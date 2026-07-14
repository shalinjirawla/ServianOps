using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.DTOs.Role;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.Services
{
    public class RoleService : Interfaces.IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly AutoMapper.IMapper _mapper;

        public RoleService(IRoleRepository roleRepository, AutoMapper.IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<RoleDto> GetRoleByIdAsync(long id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role == null ? null : _mapper.Map<RoleDto>(role);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<RoleDto>> GetRolesPagedAsync(RoleFilterDto filter)
        {
            var query = _roleRepository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                query = query.Where(r => r.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(r => r.Name.Contains(filter.Search) || r.Description.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<RoleDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<System.Collections.Generic.List<RoleDto>>(items)
            };
        }
    }
}
