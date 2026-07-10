using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.Core.Interfaces.Repositories.Jobs
{
    public interface ITradeRepository : IGenericRepository<Trade>
    {
    }

    public interface IJobRepository : IGenericRepository<Job>
    {
        Task<Job> GetJobWithDetailsAsync(long id);
        Task<IReadOnlyList<Job>> GetPagedJobsWithDetailsAsync(int pageNumber, int pageSize);
    }

    public interface IJobAttachmentRepository : IGenericRepository<JobAttachment>
    {
    }
}
