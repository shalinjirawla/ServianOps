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
        Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerTypeDto>> GetAllPagedAsync(CustomerTypeFilterDto filter);
        Task DeleteAsync(long id);
    }

    public interface ICustomerService
    {
        Task<CustomerDetailDto> CreateAsync(CreateCustomerDto dto);
        Task<CustomerDetailDto> UpdateAsync(long id, UpdateCustomerDto dto);
        Task<CustomerDetailDto> GetByIdAsync(long id);
        Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerListDto>> GetAllPagedAsync(CustomerFilterDto filter);
        Task<IReadOnlyList<DropdownDto>> GetDropdownAsync();
        Task DeleteAsync(long id);
    }

    public interface ISiteService
    {
        Task<SiteDetailDto> CreateAsync(CreateSiteDto dto);
        Task<SiteDetailDto> UpdateAsync(long id, UpdateSiteDto dto);
        Task<SiteDetailDto> GetByIdAsync(long id);
        Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<SiteListDto>> GetAllPagedAsync(SiteFilterDto filter);
        Task<IReadOnlyList<DropdownDto>> GetSitesByCustomerDropdownAsync(long customerId);
        Task DeleteAsync(long id);
    }
}
