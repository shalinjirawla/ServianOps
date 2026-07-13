using AutoMapper;
using ServianOps_Backend.Core.Entities.Identity;
using ServianOps_Backend.Application.DTOs.User;
using ServianOps_Backend.Application.DTOs.Role;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.Mappings
{
    public class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<User, UserDto>()
                .ForMember(d => d.Tenant, opt => opt.MapFrom(s => s.Tenant));
            
            CreateMap<User, UserSummaryDto>();
            CreateMap<CreateUserDto, User>();
            CreateMap<Role, RoleDto>();
            CreateMap<UserRole, RoleDto>()
                .ForMember(d => d.Id, opt => opt.MapFrom(s => s.RoleId))
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Role.Name))
                .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Role.Description));
        }
    }
}
