using System.Linq;
using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Core.Interfaces;

namespace ServianOps_Backend.Infrastructure.Multitenancy
{
    public class CurrentTenantService : ICurrentTenant
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentTenantService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public long? TenantId
        {
            get
            {
                var tenantIdClaim = _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == "tenant_id");
                if (tenantIdClaim != null && long.TryParse(tenantIdClaim.Value, out var tenantId))
                {
                    return tenantId;
                }
                
                // For scenarios like login where token is not yet generated, we might extract it from headers or body
                // if we intercept it earlier, but typically the repository will use IgnoreQueryFilters for login.
                
                return null;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
