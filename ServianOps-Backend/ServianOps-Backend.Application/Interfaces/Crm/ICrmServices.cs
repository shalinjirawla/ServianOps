using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs;
using ServianOps_Backend.Application.DTOs.Crm;

namespace ServianOps_Backend.Application.Interfaces.Crm
{
    public interface ICustomerTypeService
    {
        Task<CustomerTypeDto> CreateAsync(CreateCustomerTypeDto dto);
        Task<CustomerTypeDto> UpdateAsync(long id, UpdateCustomerTypeDto dto);
        Task<CustomerTypeDto> GetByIdAsync(long id);
        Task<IReadOnlyList<CustomerTypeDto>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task DeleteAsync(long id);
    }

    public interface ICustomerService
    {
        Task<CustomerDetailDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDetailDto> UpdateAsync(long id, UpdateCustomerDto dto);
        Task<CustomerDetailDto> GetByIdAsync(long id);
        Task<IReadOnlyList<CustomerListDto>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<IReadOnlyList<DropdownDto>> GetDropdownAsync();
        Task DeleteAsync(long id);
    }

    public interface ISiteService
    {
        Task<SiteDetailDto> CreateAsync(CreateSiteDto dto);
        Task<SiteDetailDto> UpdateAsync(long id, UpdateSiteDto dto);
        Task<SiteDetailDto> GetByIdAsync(long id);
        Task<IReadOnlyList<SiteListDto>> GetAllPagedAsync(int pageNumber, int pageSize);
        Task<IReadOnlyList<DropdownDto>> GetSitesByCustomerDropdownAsync(long customerId);
        Task DeleteAsync(long id);
    }
}
