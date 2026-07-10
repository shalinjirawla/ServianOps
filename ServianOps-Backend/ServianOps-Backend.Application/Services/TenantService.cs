using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public TenantService(ITenantRepository tenantRepository, IRoleRepository roleRepository, IUserService userService, IGenericRepository<UserRole> userRoleRepository)
        {
            _tenantRepository = tenantRepository;
            _roleRepository = roleRepository;
            _userService = userService;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<TenantDto> CreateTenantAsync(CreateTenantDto dto)
        {
            // 1. Business Validation - Check Duplicate TenancyName
            var existingTenant = await _tenantRepository.GetByTenancyNameAsync(dto.TenancyName);
            if (existingTenant != null)
            {
                throw new Exception($"TenancyName '{dto.TenancyName}' is already taken.");
            }

            // 2. Map and Save Tenant
            var tenant = new Tenant
            {
                CompanyName = dto.CompanyName,
                TenancyName = dto.TenancyName,
                PlanId = dto.PlanId,
                IsActive = true,
                LogoUrl = string.Empty,
                TimeZone = "UTC",
                Currency = "USD"
            };

            await _tenantRepository.AddAsync(tenant);

            // 3. Create Default Roles
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

            // 4. Create First Admin User
            // Typically we'd use the provided password, or default '123qwe' if not provided
            var password = string.IsNullOrWhiteSpace(dto.Password) ? "123qwe" : dto.Password;
            var createdUser = await _userService.CreateUserAsync(new DTOs.User.CreateUserDto
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = password
            }, tenant.Id);

            // 5. Assign Administrator Role to the newly created user
            if (adminRoleId > 0)
            {
                await _userRoleRepository.AddAsync(new UserRole
                {
                    TenantId = tenant.Id,
                    UserId = createdUser.Id,
                    RoleId = adminRoleId
                });
            }

            return new TenantDto
            {
                Id = tenant.Id,
                TenancyName = tenant.TenancyName,
                CompanyName = tenant.CompanyName,

                PlanId = tenant.PlanId,
                Plan = tenant.Plan != null ? new ServianOps_Backend.Application.DTOs.Plan.PlanDto 
                { 
                    Id = tenant.Plan.Id, 
                    PlanName = tenant.Plan.PlanName, 
                    MaxUsers = tenant.Plan.MaxUsers, 
                    MaxProjects = tenant.Plan.MaxProjects, 
                    MaxStorageGB = tenant.Plan.MaxStorageGB, 
                    Price = tenant.Plan.Price, 
                    BillingCycle = tenant.Plan.BillingCycle, 
                    IsTrialAvailable = tenant.Plan.IsTrialAvailable, 
                    TrialDays = tenant.Plan.TrialDays, 
                    IsActive = tenant.Plan.IsActive, 
                    CreationTime = tenant.Plan.CreationTime 
                } : null,
                IsActive = tenant.IsActive
            };
        }

        public async Task<TenantDto> GetByTenancyNameAsync(string TenancyName)
        {
            var tenant = await _tenantRepository.GetByTenancyNameAsync(TenancyName);
            if (tenant == null) return null;

            var admins = await _userService.GetTenantAdministratorsAsync(tenant.Id);

            return new TenantDto
            {
                Id = tenant.Id,
                TenancyName = tenant.TenancyName,
                CompanyName = tenant.CompanyName,

                PlanId = tenant.PlanId,
                Plan = tenant.Plan != null ? new ServianOps_Backend.Application.DTOs.Plan.PlanDto 
                { 
                    Id = tenant.Plan.Id, 
                    PlanName = tenant.Plan.PlanName, 
                    MaxUsers = tenant.Plan.MaxUsers, 
                    MaxProjects = tenant.Plan.MaxProjects, 
                    MaxStorageGB = tenant.Plan.MaxStorageGB, 
                    Price = tenant.Plan.Price, 
                    BillingCycle = tenant.Plan.BillingCycle, 
                    IsTrialAvailable = tenant.Plan.IsTrialAvailable, 
                    TrialDays = tenant.Plan.TrialDays, 
                    IsActive = tenant.Plan.IsActive, 
                    CreationTime = tenant.Plan.CreationTime 
                } : null,
                Users = admins.Select(a => new UserSummaryDto 
                { 
                    Id = a.Id, 
                    FirstName = a.FirstName, 
                    LastName = a.LastName, 
                    Email = a.Email 
                }).ToList(),
                IsActive = tenant.IsActive
            };
        }
        public async Task<IReadOnlyList<TenantDto>> GetTenantsPagedAsync(int pageNumber, int pageSize)
        {
            var tenants = await _tenantRepository.GetPagedAsync(pageNumber, pageSize);
            var result = new List<TenantDto>();

            foreach (var t in tenants)
            {
                var admins = await _userService.GetTenantAdministratorsAsync(t.Id);
                result.Add(new TenantDto
                {
                    Id = t.Id,
                    TenancyName = t.TenancyName,
                    CompanyName = t.CompanyName,
                    PlanId = t.PlanId,
                    Plan = t.Plan != null ? new ServianOps_Backend.Application.DTOs.Plan.PlanDto 
                    { 
                        Id = t.Plan.Id, 
                        PlanName = t.Plan.PlanName, 
                        MaxUsers = t.Plan.MaxUsers, 
                        MaxProjects = t.Plan.MaxProjects, 
                        MaxStorageGB = t.Plan.MaxStorageGB, 
                        Price = t.Plan.Price, 
                        BillingCycle = t.Plan.BillingCycle, 
                        IsTrialAvailable = t.Plan.IsTrialAvailable, 
                        TrialDays = t.Plan.TrialDays, 
                        IsActive = t.Plan.IsActive, 
                        CreationTime = t.Plan.CreationTime 
                    } : null,
                    Users = admins.Select(a => new UserSummaryDto 
                    { 
                        Id = a.Id, 
                        FirstName = a.FirstName, 
                        LastName = a.LastName, 
                        Email = a.Email 
                    }).ToList(),
                    IsActive = t.IsActive
                });
            }
            return result;
        }

        public async Task UpdateTenantAsync(long id, CreateTenantDto dto)
        {
            var tenant = await _tenantRepository.GetByIdAsync(id);
            if (tenant == null) throw new Exception("Tenant not found");
            tenant.CompanyName = dto.CompanyName;
            tenant.PlanId = dto.PlanId;
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
