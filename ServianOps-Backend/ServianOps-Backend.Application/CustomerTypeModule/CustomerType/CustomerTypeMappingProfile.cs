using AutoMapper;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.Application.CustomerTypeModule.CustomerType
{
    public class CustomerTypeMappingProfile : Profile
    {
        public CustomerTypeMappingProfile()
        {
            CreateMap<Core.Entities.Crm.CustomerType, CustomerTypeDetailDto>();
            CreateMap<Core.Entities.Crm.CustomerType, CustomerTypeListDto>();
            CreateMap<Core.Entities.Crm.CustomerType, CustomerTypeLookupDto>();

            CreateMap<CreateCustomerTypeDto, Core.Entities.Crm.CustomerType>();
            CreateMap<UpdateCustomerTypeDto, Core.Entities.Crm.CustomerType>();
        }
    }
}
