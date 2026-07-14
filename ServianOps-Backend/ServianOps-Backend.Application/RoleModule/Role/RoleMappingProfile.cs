using AutoMapper;
using ServianOps_Backend.Application.RoleModule.Role.RoleDto;

namespace ServianOps_Backend.Application.RoleModule.Role
{
    public class RoleMappingProfile : Profile
    {
        public RoleMappingProfile()
        {
            CreateMap<Core.Entities.Identity.Role, RoleDetailDto>();
            CreateMap<Core.Entities.Identity.Role, RoleListDto>();
            CreateMap<Core.Entities.Identity.Role, RoleLookupDto>();

            CreateMap<CreateRoleDto, Core.Entities.Identity.Role>();
            CreateMap<UpdateRoleDto, Core.Entities.Identity.Role>();
        }
    }
}
