using System.Threading.Tasks;
using ServianOps_Backend.Core.Entities.Saas;

namespace ServianOps_Backend.Core.Interfaces.Repositories
{
    public interface ITenantRepository : IGenericRepository<Tenant>
    {
        Task<Tenant> GetByTenancyNameAsync(string TenancyName);
    }
}
