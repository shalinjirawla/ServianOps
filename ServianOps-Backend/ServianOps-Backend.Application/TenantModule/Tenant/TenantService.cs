using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.Application.Interfaces;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Application.TenantModule.Tenant
{
    public class TenantService : ITenantService
    {
        private readonly IGenericRepository<Core.Entities.Saas.Tenant> _repository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMapper _mapper;

        public TenantService(
            IGenericRepository<Core.Entities.Saas.Tenant> repository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<User> userRepository,
            IPasswordHasher passwordHasher,
            IMapper mapper)
        {
            _repository = repository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }

        public async Task<StandardResponse<TenantDetailDto>> CreateTenant(CreateTenantDto dto)
        {
            var entity = _mapper.Map<Core.Entities.Saas.Tenant>(dto);
            
            // Default values for required properties
            entity.LogoUrl = string.Empty;
            entity.TimeZone = "UTC";
            entity.Currency = "USD";

            await _repository.AddAsync(entity); // Gets Tenant Id

            // Create default roles
            var adminRole = new Role { TenantId = entity.Id, Name = "Administrator", Description = "System Administrator" };
            var plannerRole = new Role { TenantId = entity.Id, Name = "Planner", Description = "Planner" };
            var engineerRole = new Role { TenantId = entity.Id, Name = "Engineer", Description = "Engineer" };

            await _roleRepository.AddAsync(adminRole);
            await _roleRepository.AddAsync(plannerRole);
            await _roleRepository.AddAsync(engineerRole);

            // Hash password (default to '123qwe' if not provided)
            var password = string.IsNullOrEmpty(dto.Password) ? "123qwe" : dto.Password;
            var hash = _passwordHasher.HashPassword(password, out var salt);

            // Create admin user using AutoMapper
            var adminUser = _mapper.Map<User>(dto);
            adminUser.TenantId = entity.Id;
            adminUser.PasswordHash = hash;
            adminUser.PasswordSalt = salt;
            adminUser.IsActive = true;
            adminUser.ProfileImage = string.Empty;
            adminUser.Phone = adminUser.Phone ?? string.Empty;
            adminUser.UserRoles = new List<UserRole>
            {
                new UserRole { RoleId = adminRole.Id, TenantId = entity.Id }
            };
            await _userRepository.AddAsync(adminUser);

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
