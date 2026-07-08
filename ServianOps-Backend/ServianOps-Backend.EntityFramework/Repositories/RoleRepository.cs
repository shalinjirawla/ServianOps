using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Core.Interfaces.Repositories;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
