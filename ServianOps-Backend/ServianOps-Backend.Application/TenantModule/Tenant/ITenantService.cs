using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;

namespace ServianOps_Backend.Application.TenantModule.Tenant
{
    public interface ITenantService
    {
        Task<StandardResponse<TenantDetailDto>> CreateTenant(CreateTenantDto dto);
        Task<StandardResponse<TenantDetailDto>> UpdateTenant(long id, UpdateTenantDto dto);
        Task<StandardResponse<TenantDetailDto>> GetTenantById(long id);
        Task<StandardResponse<TenantDetailDto>> GetByTenancyName(string tenancyName);
        Task<StandardResponse<PagedResultDto<TenantListDto>>> GetAllTenants(TenantFilterDto filter);
        Task<StandardResponse<IReadOnlyList<TenantLookupDto>>> GetTenantLookup();
        Task<StandardResponse<bool>> DeleteTenant(long id);
        
        // Setup default tenant for system
        Task<StandardResponse<bool>> SetupDefaultTenant();
    }
}
