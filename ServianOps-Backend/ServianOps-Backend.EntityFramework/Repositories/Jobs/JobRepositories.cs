using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Core.Interfaces.Repositories.Jobs;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories.Jobs
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        public TradeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class JobRepository : GenericRepository<Job>, IJobRepository
    {
        public JobRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Job> GetJobWithDetailsAsync(long id)
        {
            return await _dbContext.Jobs
                .Include(x => x.Customer)
                .Include(x => x.Site)
                .Include(x => x.Trade)
                .Include(x => x.Attachments)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IReadOnlyList<Job>> GetPagedJobsWithDetailsAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Jobs
                .Include(x => x.Customer)
                .Include(x => x.Site)
                .Include(x => x.Trade)
                .OrderByDescending(x => x.CreationTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }

    public class JobAttachmentRepository : GenericRepository<JobAttachment>, IJobAttachmentRepository
    {
        public JobAttachmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
