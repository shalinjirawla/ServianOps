using System.Linq;
using AutoMapper;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.Application.CustomerModule.Customer
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<Core.Entities.Crm.Customer, CustomerDetailDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.Contacts, opt => opt.MapFrom(s => s.CustomerContacts));
                
            CreateMap<Core.Entities.Crm.Customer, CustomerListDto>()
                .ForMember(d => d.AccountManagerName, opt => opt.MapFrom(s => s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null))
                .ForMember(d => d.PrimaryContactName, opt => opt.MapFrom(s => s.CustomerContacts.FirstOrDefault(c => c.IsActive) != null ? $"{s.CustomerContacts.FirstOrDefault(c => c.IsActive).FirstName} {s.CustomerContacts.FirstOrDefault(c => c.IsActive).LastName}" : null))
                .ForMember(d => d.PrimaryContactMobile, opt => opt.MapFrom(s => s.CustomerContacts.FirstOrDefault(c => c.IsActive) != null ? s.CustomerContacts.FirstOrDefault(c => c.IsActive).MobileNumber : null));
            CreateMap<Core.Entities.Crm.Customer, CustomerLookupDto>();
            
            CreateMap<CreateCustomerDto, Core.Entities.Crm.Customer>()
                .ForMember(d => d.CustomerContacts, opt => opt.MapFrom(s => new System.Collections.Generic.List<Core.Entities.Crm.CustomerContact> 
                { 
                    new Core.Entities.Crm.CustomerContact 
                    { 
                        FirstName = s.ContactFirstName, 
                        LastName = s.ContactLastName, 
                        MobileNumber = s.ContactMobile, 
                        Email = s.ContactEmail, 
                        IsActive = true 
                    } 
                }));
            CreateMap<UpdateCustomerDto, Core.Entities.Crm.Customer>();
            
            CreateMap<CustomerContact, CustomerContactDto>().ReverseMap();
        }
    }
}
