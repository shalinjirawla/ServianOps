using System.Threading.Tasks;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Core.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByEmailAndTenantIdAsync(string email, long? tenantId);
        Task<string> GetUserRoleNameAsync(long userId);
        Task<System.Collections.Generic.IReadOnlyList<User>> GetAdministratorsPagedAsync(int pageNumber, int pageSize);
        Task<System.Collections.Generic.IReadOnlyList<User>> GetTenantAdministratorsAsync(long tenantId);
    }
}
