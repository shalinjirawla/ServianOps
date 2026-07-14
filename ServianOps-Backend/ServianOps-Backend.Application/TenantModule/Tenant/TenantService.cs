using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.TenantModule.Tenant
{
    public class TenantService : ITenantService
    {
        private readonly IGenericRepository<Core.Entities.Saas.Tenant> _repository;
        private readonly IMapper _mapper;

        public TenantService(IGenericRepository<Core.Entities.Saas.Tenant> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<TenantDetailDto>> CreateTenant(CreateTenantDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Saas.Tenant>(dto);
            await _repository.AddAsync(entity);
            return StandardResponse<TenantDetailDto>.Ok(_mapper.Map<TenantDetailDto>(entity));
        }

        public async Task<StandardResponse<TenantDetailDto>> UpdateTenant(long id, UpdateTenantDto dto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return StandardResponse<TenantDetailDto>.Error("Tenant not found.");

            _mapper.Map(dto, entity);
            await _repository.UpdateAsync(entity);

            return StandardResponse<TenantDetailDto>.Ok(_mapper.Map<TenantDetailDto>(entity));
        }

        public async Task<StandardResponse<TenantDetailDto>> GetTenantById(long id)
        {
            var entity = await _repository.GetQueryable()
                .Include(t => t.Plan)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) return StandardResponse<TenantDetailDto>.Error("Tenant not found.");
            
            return StandardResponse<TenantDetailDto>.Ok(_mapper.Map<TenantDetailDto>(entity));
        }

        public async Task<StandardResponse<TenantDetailDto>> GetByTenancyName(string tenancyName)
        {
            var entity = await _repository.GetQueryable()
                .FirstOrDefaultAsync(t => t.TenancyName == tenancyName);
            if (entity == null) return StandardResponse<TenantDetailDto>.Error("Tenant not found.");
            
            return StandardResponse<TenantDetailDto>.Ok(_mapper.Map<TenantDetailDto>(entity));
        }

        public async Task<StandardResponse<PagedResultDto<TenantListDto>>> GetAllTenants(TenantFilterDto filter)
        {
            var query = _repository.GetQueryable()
                .Include(t => t.Plan)
                .Include(t => t.Users)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (filter.IsActive.HasValue)
            {
                query = query.Where(t => t.IsActive == filter.IsActive.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(t => t.CompanyName.Contains(filter.Search) || t.TenancyName.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<TenantListDto>(
                _mapper.Map<IReadOnlyList<TenantListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<TenantListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<TenantLookupDto>>> GetTenantLookup()
        {
            var items = await _repository.GetAllAsync();
            var result = items.Select(t => new TenantLookupDto
            {
                Id = t.Id,
                CompanyName = t.CompanyName,
                TenancyName = t.TenancyName
            }).OrderBy(x => x.CompanyName).ToList();

            return StandardResponse<IReadOnlyList<TenantLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteTenant(long id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                await _repository.DeleteAsync(entity);
                return StandardResponse<bool>.Ok(true, "Tenant deleted.");
            }
            return StandardResponse<bool>.Error("Tenant not found.");
        }
        
        public async Task<StandardResponse<bool>> SetupDefaultTenant()
        {
            var hasTenants = await _repository.GetQueryable().AnyAsync();
            if (!hasTenants)
            {
                var defaultTenant = new Core.Entities.Saas.Tenant
                {
                    CompanyName = "ServianOps Default",
                    TenancyName = "default",
                    IsActive = true
                };
                await _repository.AddAsync(defaultTenant);
                return StandardResponse<bool>.Ok(true, "Default tenant created.");
            }
            return StandardResponse<bool>.Ok(true, "Tenant already exists.");
        }
    }
}
