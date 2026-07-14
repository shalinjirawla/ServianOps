using System.Linq;
using AutoMapper;
using ServianOps_Backend.Application.UserModule.User.UserDto;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Application.UserModule.User
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<Core.Entities.Identity.User, UserDetailDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.TenancyName : null))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToArray()));

            CreateMap<Core.Entities.Identity.User, UserListDto>()
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.TenancyName : null))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToArray()));

            CreateMap<Core.Entities.Identity.User, UserLookupDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToArray()));

            CreateMap<CreateUserDto, Core.Entities.Identity.User>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => false));

            CreateMap<UpdateUserDto, Core.Entities.Identity.User>();
        }
    }
}
