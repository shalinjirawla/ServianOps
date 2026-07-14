using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Tenant;
using ServianOps_Backend.Application.DTOs.Shared;
namespace ServianOps_Backend.Application.Interfaces
{
    public interface ITenantService
    {
        Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);
        Task<TenantDto> GetByTenancyNameAsync(string TenancyName);
        Task<TenantDto> GetTenantByIdAsync(long id);
        Task<PagedResponseDto<TenantDto>> GetTenantsPagedAsync(TenantFilterDto filter);
        Task UpdateTenantAsync(long id, CreateTenantDto dto);
        Task DeleteTenantAsync(long id);
    }
}