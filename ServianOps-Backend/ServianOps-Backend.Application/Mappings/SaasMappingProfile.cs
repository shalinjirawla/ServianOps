using AutoMapper;
using ServianOps_Backend.Core.Entities.Saas;
using ServianOps_Backend.Application.DTOs.Tenant;
using ServianOps_Backend.Application.DTOs.Plan;
using ServianOps_Backend.Application.DTOs.Shared;
using System.Linq;

namespace ServianOps_Backend.Application.Mappings
{
    public class SaasMappingProfile : Profile
    {
        public SaasMappingProfile()
        {
            CreateMap<Tenant, TenantDto>()
                .ForMember(d => d.Plan, opt => opt.MapFrom(s => s.Plan))
                .ForMember(d => d.Users, opt => opt.MapFrom(s => s.Users));
            
            CreateMap<Tenant, TenantSummaryDto>();
            CreateMap<CreateTenantDto, Tenant>();
            
            CreateMap<Plan, PlanDto>();
            CreateMap<Plan, PlanSummaryDto>();
            CreateMap<CreatePlanDto, Plan>();
        }
    }
}
