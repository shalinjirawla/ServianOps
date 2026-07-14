using AutoMapper;
using ServianOps_Backend.Application.JobModule.Job.JobDto;
using ServianOps_Backend.Core.Entities.Jobs;

namespace ServianOps_Backend.Application.JobModule.Job
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<Core.Entities.Jobs.Job, JobDetailDto>();
            CreateMap<Core.Entities.Jobs.Job, JobListDto>();
            CreateMap<JobAttachment, JobAttachmentDto>();

            CreateMap<CreateJobDto, Core.Entities.Jobs.Job>()
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());

            CreateMap<UpdateJobDto, Core.Entities.Jobs.Job>()
                .ForMember(dest => dest.Attachments, opt => opt.Ignore());
        }
    }
}
