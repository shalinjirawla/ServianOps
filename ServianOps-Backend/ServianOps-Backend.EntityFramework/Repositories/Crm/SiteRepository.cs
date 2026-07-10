using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories.Crm
{
    public class SiteRepository : GenericRepository<Site>, ISiteRepository
    {
        public SiteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IReadOnlyList<Site>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Sites
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts.Where(contact => contact.IsActive))
                .AsNoTracking()
                .OrderByDescending(s => s.CreationTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Site> GetSiteWithContactsAsync(long id)
        {
            return await _dbContext.Sites
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts.Where(contact => contact.IsActive && !contact.IsDeleted))
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public override async Task UpdateAsync(Site entity)
        {
            _dbContext.Sites.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
