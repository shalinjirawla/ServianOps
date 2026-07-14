using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;

namespace ServianOps_Backend.Application.CustomerTypeModule.CustomerType
{
    public interface ICustomerTypeService
    {
        Task<StandardResponse<CustomerTypeDetailDto>> CreateCustomerType(CreateCustomerTypeDto dto);
        Task<StandardResponse<CustomerTypeDetailDto>> UpdateCustomerType(long id, UpdateCustomerTypeDto dto);
        Task<StandardResponse<CustomerTypeDetailDto>> GetCustomerTypeById(long id);
        Task<StandardResponse<PagedResultDto<CustomerTypeListDto>>> GetAllCustomerTypes(CustomerTypeFilterDto filter);
        Task<StandardResponse<IReadOnlyList<CustomerTypeLookupDto>>> GetCustomerTypeLookup();
        Task<StandardResponse<bool>> DeleteCustomerType(long id);
    }
}
