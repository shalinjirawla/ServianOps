using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.DTOs.Tenant;
using ServianOps_Backend.Application.DTOs.Shared;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.Application.Interfaces;

namespace ServianOps_Backend.Application.Services
{
    public class TenantService : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserService _userService;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public TenantService(
            ITenantRepository tenantRepository, 
            IRoleRepository roleRepository, 
            IUserService userService, 
            IGenericRepository<UserRole> userRoleRepository,
            IMapper mapper)
        {
            _tenantRepository = tenantRepository;
            _roleRepository = roleRepository;
            _userService = userService;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
        {
            var existingTenant = await _tenantRepository.GetByTenancyNameAsync(dto.TenancyName);
            if (existingTenant != null)
            {
                throw new Exception($"TenancyName '{dto.TenancyName}' is already taken.");
            }

            var tenant = _mapper.Map<Tenant>(dto);
            tenant.IsActive = true;
            tenant.LogoUrl = string.Empty;
            tenant.TimeZone = "UTC";
            tenant.Currency = "USD";

            await _tenantRepository.AddAsync(tenant);

            string[] defaultRoles = { "Administrator", "Costing User", "Engineer", "Job Desk User" };
            long adminRoleId = 0;
            foreach (var roleName in defaultRoles)
            {
                var role = new Role
                {
                    TenantId = tenant.Id,
                    Name = roleName,
                    Description = $"Default {roleName} role",
                    IsActive = true
                };
                await _roleRepository.AddAsync(role);
                
                if (roleName == "Administrator")
                {
                    adminRoleId = role.Id;
                }
            }

            var password = string.IsNullOrWhiteSpace(dto.Password) ? "123qwe" : dto.Password;
            var createdUser = await _userService.CreateUserAsync(new DTOs.User.CreateUserDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = password
            }, tenant.Id);

            if (adminRoleId > 0)
            {
                await _userRoleRepository.AddAsync(new UserRole
                {
                    TenantId = tenant.Id,
                    UserId = createdUser.Id,
                    RoleId = adminRoleId
                });
            }

            // Fetch fully populated tenant
            var fullTenant = await _tenantRepository.GetQueryable()
                .Include(t => t.Plan)
                .Include(t => t.Users)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(t => t.Id == tenant.Id);

            var tenantDto = _mapper.Map<TenantDto>(fullTenant);
            tenantDto.Users = fullTenant.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Administrator"))
                .Select(u => _mapper.Map<UserSummaryDto>(u))
                .ToList();

            return tenantDto;
        }

        public async Task<TenantDto> GetByTenancyNameAsync(string TenancyName)
        {
            var tenant = await _tenantRepository.GetQueryable()
                .Include(t => t.Plan)
                .Include(t => t.Users)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(t => t.TenancyName == TenancyName);

            if (tenant == null) return null;

            var tenantDto = _mapper.Map<TenantDto>(tenant);
            tenantDto.Users = tenant.Users
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Administrator"))
                .Select(u => _mapper.Map<UserSummaryDto>(u))
                .ToList();

            return tenantDto;
        }

        public async Task<IReadOnlyList<TenantDto>> GetTenantsPagedAsync(int pageNumber, int pageSize)
        {
            var tenants = await _tenantRepository.GetQueryable()
                .Include(t => t.Plan)
                .Include(t => t.Users)
                    .ThenInclude(u => u.UserRoles)
                        .ThenInclude(ur => ur.Role)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<TenantDto>();

            foreach (var t in tenants)
            {
                var dto = _mapper.Map<TenantDto>(t);
                dto.Users = t.Users
                    .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Administrator"))
                    .Select(u => _mapper.Map<UserSummaryDto>(u))
                    .ToList();
                result.Add(dto);
            }
            return result;
        }

        public async Task UpdateTenantAsync(long id, CreateTenantDto dto)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null) throw new Exception("Tenant not found");
            
            _mapper.Map(dto, tenant);
            await _tenantRepository.UpdateAsync(tenant);
        }

        public async Task DeleteTenantAsync(long id)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null) throw new Exception("Tenant not found");
            await _tenantRepository.DeleteAsync(tenant);
        }
    }
}
