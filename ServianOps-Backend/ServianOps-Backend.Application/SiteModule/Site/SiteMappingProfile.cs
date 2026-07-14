using System.Linq;
using AutoMapper;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.Application.SiteModule.Site
{
    public class SiteMappingProfile : Profile
    {
        public SiteMappingProfile()
        {
            CreateMap<Core.Entities.Crm.Site, SiteDetailDto>()
                .ForMember(dest => dest.AccountManagerName, opt => opt.MapFrom(src => 
                    src.AccountManager != null ? $"{src.AccountManager.FirstName} {src.AccountManager.LastName}" : null))
                .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.SiteContacts));

            CreateMap<Core.Entities.Crm.Site, SiteListDto>()
                .ForMember(dest => dest.AccountManagerName, opt => opt.MapFrom(src => 
                    src.AccountManager != null ? $"{src.AccountManager.FirstName} {src.AccountManager.LastName}" : null))
                .ForMember(dest => dest.PrimaryContactName, opt => opt.MapFrom(src => 
                    src.SiteContacts != null && src.SiteContacts.Any() ? $"{src.SiteContacts.First().FirstName} {src.SiteContacts.First().LastName}" : null))
                .ForMember(dest => dest.PrimaryContactMobile, opt => opt.MapFrom(src => 
                    src.SiteContacts != null && src.SiteContacts.Any() ? src.SiteContacts.First().MobileNumber : null));

            CreateMap<Core.Entities.Crm.Site, SiteLookupDto>();

            CreateMap<CreateSiteDto, Core.Entities.Crm.Site>();
            CreateMap<UpdateSiteDto, Core.Entities.Crm.Site>();

            CreateMap<SiteContact, SiteContactDto>();
        }
    }
}
