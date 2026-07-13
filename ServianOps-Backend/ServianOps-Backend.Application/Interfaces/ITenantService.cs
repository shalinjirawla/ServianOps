using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Tenant;
namespace ServianOps_Backend.Application.Interfaces
{
    public interface ITenantService
    {
        Task<TenantDto> CreateTenantAsync(CreateTenantDto dto);
        Task<TenantDto> GetByTenancyNameAsync(string TenancyName);
        Task<TenantDto> GetTenantByIdAsync(long id);
        Task<IReadOnlyList<TenantDto>> GetTenantsPagedAsync(int pageNumber, int pageSize);
        Task UpdateTenantAsync(long id, CreateTenantDto dto);
        Task DeleteTenantAsync(long id);
    }
}