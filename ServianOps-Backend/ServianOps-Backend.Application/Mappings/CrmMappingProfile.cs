using System.Linq;
using AutoMapper;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Application.DTOs.Crm;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.Mappings
{
    public class CrmMappingProfile : Profile
    {
        public CrmMappingProfile()
        {
            CreateMap<CustomerType, CustomerTypeDto>();
            CreateMap<CreateCustomerTypeDto, CustomerType>();

            CreateMap<Customer, CustomerSummaryDto>();
            CreateMap<Customer, CustomerDetailDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.Contacts, opt => opt.MapFrom(s => s.CustomerContacts));
            
            CreateMap<Customer, CustomerListDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.PrimaryContactName, opt => opt.MapFrom(s => s.CustomerContacts.FirstOrDefault(c => c.IsActive) != null ? $"{s.CustomerContacts.FirstOrDefault(c => c.IsActive).FirstName} {s.CustomerContacts.FirstOrDefault(c => c.IsActive).LastName}" : null))
                .ForMember(d => d.PrimaryContactMobile, opt => opt.MapFrom(s => s.CustomerContacts.FirstOrDefault(c => c.IsActive) != null ? s.CustomerContacts.FirstOrDefault(c => c.IsActive).MobileNumber : null));

            CreateMap<CreateCustomerDto, Customer>()
                .ForMember(d => d.CustomerContacts, opt => opt.MapFrom(s => new System.Collections.Generic.List<CustomerContact> 
                { 
                    new CustomerContact 
                    { 
                        FirstName = s.ContactFirstName, 
                        LastName = s.ContactLastName, 
                        MobileNumber = s.ContactMobile, 
                        Email = s.ContactEmail, 
                        IsActive = true 
                    } 
                }));

            CreateMap<CustomerContact, CustomerContactDto>();

            CreateMap<Site, SiteSummaryDto>();
            CreateMap<Site, SiteDetailDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.Contacts, opt => opt.MapFrom(s => s.SiteContacts));

            CreateMap<Site, SiteListDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.PrimaryContactName, opt => opt.MapFrom(s => s.SiteContacts.FirstOrDefault(c => c.IsActive) != null ? $"{s.SiteContacts.FirstOrDefault(c => c.IsActive).FirstName} {s.SiteContacts.FirstOrDefault(c => c.IsActive).LastName}" : null))
                .ForMember(d => d.PrimaryContactMobile, opt => opt.MapFrom(s => s.SiteContacts.FirstOrDefault(c => c.IsActive) != null ? s.SiteContacts.FirstOrDefault(c => c.IsActive).MobileNumber : null));

            CreateMap<CreateSiteDto, Site>()
                .ForMember(d => d.SiteContacts, opt => opt.MapFrom(s => new System.Collections.Generic.List<SiteContact> 
                { 
                    new SiteContact 
                    { 
                        FirstName = s.ContactFirstName, 
                        LastName = s.ContactLastName, 
                        MobileNumber = s.ContactMobile, 
                        Email = s.ContactEmail, 
                        IsActive = true 
                    } 
                }));
                
            CreateMap<SiteContact, SiteContactDto>();
        }
    }
}
