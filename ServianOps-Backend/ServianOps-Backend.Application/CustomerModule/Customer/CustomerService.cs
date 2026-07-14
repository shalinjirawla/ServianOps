using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.CustomerModule.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly IGenericRepository<Core.Entities.Crm.Customer> _repository;
        private readonly IMapper _mapper;

        public CustomerService(IGenericRepository<Core.Entities.Crm.Customer> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<CustomerDetailDto>> CreateCustomer(CreateCustomerDto dto)
        {
            var customer = _mapper.Map<Core.Entities.Crm.Customer>(dto);

            // Ensure contact list exists
            customer.CustomerContacts ??= new List<CustomerContact>();
            if (!string.IsNullOrWhiteSpace(dto.ContactFirstName) || !string.IsNullOrWhiteSpace(dto.ContactLastName) || !string.IsNullOrWhiteSpace(dto.ContactEmail) || !string.IsNullOrWhiteSpace(dto.ContactMobile))
            {
                customer.CustomerContacts.Add(new CustomerContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    MobileNumber = dto.ContactMobile,
                    Email = dto.ContactEmail,
                    IsActive = true
                });
            }

            await _repository.AddAsync(customer);

            var savedCustomer = await _repository.GetQueryable()
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            return StandardResponse<CustomerDetailDto>.Ok(_mapper.Map<CustomerDetailDto>(savedCustomer));
        }

        public async Task<StandardResponse<CustomerDetailDto>> UpdateCustomer(long id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetQueryable()
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (customer == null) 
                return StandardResponse<CustomerDetailDto>.Error("Customer not found.");

            _mapper.Map(dto, customer);

            var contact = customer.CustomerContacts.FirstOrDefault(c => dto.PrimaryContactId == null || c.Id == dto.PrimaryContactId);
            
            if (contact != null)
            {
                contact.FirstName = dto.ContactFirstName;
                contact.LastName = dto.ContactLastName;
                contact.MobileNumber = dto.ContactMobile;
                contact.Email = dto.ContactEmail;
            }
            else if (!string.IsNullOrWhiteSpace(dto.ContactFirstName) || !string.IsNullOrWhiteSpace(dto.ContactLastName) || !string.IsNullOrWhiteSpace(dto.ContactEmail) || !string.IsNullOrWhiteSpace(dto.ContactMobile))
            {
                customer.CustomerContacts.Add(new CustomerContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    MobileNumber = dto.ContactMobile,
                    Email = dto.ContactEmail,
                    IsActive = true
                });
            }

            await _repository.UpdateAsync(customer);

            var updatedCustomer = await _repository.GetQueryable()
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            return StandardResponse<CustomerDetailDto>.Ok(_mapper.Map<CustomerDetailDto>(updatedCustomer));
        }

        public async Task<StandardResponse<CustomerDetailDto>> GetCustomerById(long id)
        {
            var customer = await _repository.GetQueryable()
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return StandardResponse<CustomerDetailDto>.Error("Customer not found.");

            return StandardResponse<CustomerDetailDto>.Ok(_mapper.Map<CustomerDetailDto>(customer));
        }

        public async Task<StandardResponse<PagedResultDto<CustomerListDto>>> GetAllCustomers(CustomerFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                // Customer entity might not have IsActive directly, filtering is omitted or needs to be based on an existing property
            }

            if (filter.CustomerTypeId.HasValue)
            {
                query = query.Where(c => c.CustomerTypeId == filter.CustomerTypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(c => c.Name.Contains(filter.Search) || c.CompanyName.Contains(filter.Search) || c.AccountNumber.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var customers = await query
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            
            var result = new PagedResultDto<CustomerListDto>(
                _mapper.Map<IReadOnlyList<CustomerListDto>>(customers),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<CustomerListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<CustomerLookupDto>>> GetCustomerLookup()
        {
            var customers = await _repository.GetAllAsync();
            var result = customers.Select(c => new CustomerLookupDto
            {
                Id = c.Id,
                Name = string.IsNullOrWhiteSpace(c.CompanyName) ? c.Name : c.CompanyName
            }).OrderBy(x => x.Name).ToList();

            return StandardResponse<IReadOnlyList<CustomerLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteCustomer(long id)
        {
            var customer = await _repository.GetQueryable()
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (customer != null)
            {
                foreach (var contact in customer.CustomerContacts)
                {
                    contact.IsDeleted = true;
                    contact.IsActive = false;
                    contact.DeletedDate = DateTime.UtcNow;
                }
                await _repository.DeleteAsync(customer);
                return StandardResponse<bool>.Ok(true, "Customer deleted successfully.");
            }
            return StandardResponse<bool>.Error("Customer not found.");
        }
    }
}
