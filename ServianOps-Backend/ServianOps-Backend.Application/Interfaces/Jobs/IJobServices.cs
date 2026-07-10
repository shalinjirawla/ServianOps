using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Jobs;

namespace ServianOps_Backend.Application.Interfaces.Jobs
{
    public interface ITradeService
    {
        Task<TradeDto> CreateAsync(CreateTradeDto dto);
        Task<TradeDto> UpdateAsync(long id, UpdateTradeDto dto);
        Task<TradeDto> GetByIdAsync(long id);
        Task<IReadOnlyList<TradeListDto>> GetAllPagedAsync(int pageNumber, int pageSize, string searchTerm);
        Task DeleteAsync(long id);
    }

    public interface IJobService
    {
        Task<JobDetailDto> CreateAsync(CreateJobDto dto);
        Task<JobDetailDto> UpdateAsync(long id, UpdateJobDto dto);
        Task<JobDetailDto> GetByIdAsync(long id);
        Task<IReadOnlyList<JobListDto>> GetAllPagedAsync(int pageNumber, int pageSize, string searchTerm);
        Task DeleteAsync(long id);
        Task DeleteAttachmentAsync(long jobId, long attachmentId);
    }
}
