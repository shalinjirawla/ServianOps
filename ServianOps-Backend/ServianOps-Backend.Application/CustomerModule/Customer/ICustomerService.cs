using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;

namespace ServianOps_Backend.Application.CustomerModule.Customer
{
    public interface ICustomerService
    {
        Task<StandardResponse<CustomerDetailDto>> CreateCustomer(CreateCustomerDto dto);
        Task<StandardResponse<CustomerDetailDto>> UpdateCustomer(long id, UpdateCustomerDto dto);
        Task<StandardResponse<CustomerDetailDto>> GetCustomerById(long id);
        Task<StandardResponse<PagedResultDto<CustomerListDto>>> GetAllCustomers(CustomerFilterDto filter);
        Task<StandardResponse<IReadOnlyList<CustomerLookupDto>>> GetCustomerLookup();
        Task<StandardResponse<bool>> DeleteCustomer(long id);
    }
}
