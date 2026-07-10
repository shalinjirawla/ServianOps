using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<System.Collections.Generic.IReadOnlyList<User>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Users
                .Include(u => u.Tenant)
                .AsNoTracking()
                .OrderByDescending(u => u.CreationTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public override async Task<User> GetByIdAsync(long id)
        {
            return await _dbContext.Users
                .Include(u => u.Tenant)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            // This will automatically be filtered by TenantId due to Global Query Filter!
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByEmailAndTenantIdAsync(string email, long? tenantId)
        {
            return await _dbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == email && u.TenantId == tenantId && !u.IsDeleted);
        }

        public async Task<string> GetUserRoleNameAsync(long userId)
        {
            var roleName = await (from ur in _dbContext.UserRoles
                                  join r in _dbContext.Roles on ur.RoleId equals r.Id
                                  where ur.UserId == userId && !ur.IsDeleted && !r.IsDeleted
                                  select r.Name).FirstOrDefaultAsync();
            return roleName;
        }

        public async Task<System.Collections.Generic.IReadOnlyList<User>> GetAdministratorsPagedAsync(int pageNumber, int pageSize)
        {
            var query = from u in _dbContext.Users
                        join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                        join r in _dbContext.Roles on ur.RoleId equals r.Id
                        where r.Name == "Administrator" && !u.IsDeleted && !ur.IsDeleted && !r.IsDeleted
                        select u;

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<System.Collections.Generic.IReadOnlyList<User>> GetTenantAdministratorsAsync(long tenantId)
        {
            var query = from u in _dbContext.Users.IgnoreQueryFilters()
                        join ur in _dbContext.UserRoles.IgnoreQueryFilters() on u.Id equals ur.UserId
                        join r in _dbContext.Roles.IgnoreQueryFilters() on ur.RoleId equals r.Id
                        where r.Name == "Administrator" 
                              && u.TenantId == tenantId 
                              && !u.IsDeleted 
                              && !ur.IsDeleted 
                              && !r.IsDeleted
                        select u;

            return await query.ToListAsync();
        }
    }
}
