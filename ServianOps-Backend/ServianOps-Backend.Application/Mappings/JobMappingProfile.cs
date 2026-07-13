using AutoMapper;
using ServianOps_Backend.Core.Entities.Jobs;
using ServianOps_Backend.Application.DTOs.Jobs;

namespace ServianOps_Backend.Application.Mappings
{
    public class JobMappingProfile : Profile
    {
        public JobMappingProfile()
        {
            CreateMap<Trade, TradeDto>();
            CreateMap<Trade, TradeListDto>();
            CreateMap<CreateTradeDto, Trade>();

            CreateMap<Job, JobDetailDto>();
            CreateMap<Job, JobListDto>();
            CreateMap<CreateJobDto, Job>()
                .ForMember(d => d.Attachments, opt => opt.Ignore());

            CreateMap<JobAttachment, JobAttachmentDto>();
        }
    }
}
