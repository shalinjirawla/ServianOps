using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories
{
    public class TenantRepository : GenericRepository<Tenant>, ITenantRepository
    {
        public TenantRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Tenant> GetByTenancyNameAsync(string TenancyName)
        {
            // Specifically bypass global query filters if resolving tenant during login/middleware.
            // IgnoreQueryFilters is critical here because if the tenant is not yet resolved, 
            // the global query filter would block it.
            return await _dbContext.Tenants
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.TenancyName == TenancyName);
        }
    }
}
