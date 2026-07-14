using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.RoleModule.Role.RoleDto;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.RoleModule.Role
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Core.Entities.Identity.Role> _repository;
        private readonly IMapper _mapper;

        public RoleService(IGenericRepository<Core.Entities.Identity.Role> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<RoleDetailDto>> CreateRole(CreateRoleDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Identity.Role>(dto);
            await _repository.AddAsync(entity);
            return StandardResponse<RoleDetailDto>.Ok(_mapper.Map<RoleDetailDto>(entity));
        }

        public async Task<StandardResponse<RoleDetailDto>> UpdateRole(long id, UpdateRoleDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<RoleDetailDto>.Error("Role not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return StandardResponse<RoleDetailDto>.Ok(_mapper.Map<RoleDetailDto>(entity));
        }

        public async Task<StandardResponse<RoleDetailDto>> GetRoleById(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<RoleDetailDto>.Error("Role not found.");
            
            return StandardResponse<RoleDetailDto>.Ok(_mapper.Map<RoleDetailDto>(entity));
        }

        public async Task<StandardResponse<PagedResultDto<RoleListDto>>> GetAllRoles(RoleFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(r => r.Name.Contains(filter.Search) || r.Description.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<RoleListDto>(
                _mapper.Map<IReadOnlyList<RoleListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<RoleListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<RoleLookupDto>>> GetRoleLookup()
        {
            var items = await _repository.GetAllAsync();
            var result = items.Select(r => new RoleLookupDto
            {
                Id = r.Id,
                Name = r.Name
            }).OrderBy(x => x.Name).ToList();

            return StandardResponse<IReadOnlyList<RoleLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteRole(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return StandardResponse<bool>.Ok(true, "Role deleted.");
            }
            return StandardResponse<bool>.Error("Role not found.");
        }
    }
}
