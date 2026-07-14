using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.JobModule.Job.JobDto;

namespace ServianOps_Backend.Application.JobModule.Job
{
    public interface IJobService
    {
        Task<StandardResponse<JobDetailDto>> CreateJob(CreateJobDto dto);
        Task<StandardResponse<JobDetailDto>> UpdateJob(long id, UpdateJobDto dto);
        Task<StandardResponse<JobDetailDto>> GetJobById(long id);
        Task<StandardResponse<PagedResultDto<JobListDto>>> GetAllJobs(JobFilterDto filter);
        Task<StandardResponse<bool>> DeleteJob(long id);
        Task<StandardResponse<bool>> DeleteAttachment(long jobId, long attachmentId);
    }
}
