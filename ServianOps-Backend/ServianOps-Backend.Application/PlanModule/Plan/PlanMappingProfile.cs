using AutoMapper;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;

namespace ServianOps_Backend.Application.PlanModule.Plan
{
    public class PlanMappingProfile : Profile
    {
        public PlanMappingProfile()
        {
            CreateMap<Core.Entities.Saas.Plan, PlanDetailDto>();
            CreateMap<Core.Entities.Saas.Plan, PlanListDto>();
            CreateMap<Core.Entities.Saas.Plan, PlanLookupDto>();

            CreateMap<CreatePlanDto, Core.Entities.Saas.Plan>();
            CreateMap<UpdatePlanDto, Core.Entities.Saas.Plan>();
        }
    }
}
