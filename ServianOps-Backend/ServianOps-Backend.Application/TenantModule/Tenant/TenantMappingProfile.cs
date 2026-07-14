using AutoMapper;
using ServianOps_Backend.Application.TenantModule.Tenant.TenantDto;

namespace ServianOps_Backend.Application.TenantModule.Tenant
{
    public class TenantMappingProfile : Profile
    {
        public TenantMappingProfile()
        {
            CreateMap<Core.Entities.Saas.Tenant, TenantDetailDto>();
            CreateMap<Core.Entities.Saas.Tenant, TenantListDto>()
                .ForMember(dest => dest.AdminUser, opt => opt.MapFrom(src => 
                    System.Linq.Enumerable.FirstOrDefault(src.Users, u => System.Linq.Enumerable.Any(u.UserRoles, ur => ur.Role.Name.ToLower() == "administrator"))));
            CreateMap<Core.Entities.Saas.Tenant, TenantLookupDto>();

            CreateMap<CreateTenantDto, Core.Entities.Saas.Tenant>();
            CreateMap<UpdateTenantDto, Core.Entities.Saas.Tenant>();
        }
    }
}
