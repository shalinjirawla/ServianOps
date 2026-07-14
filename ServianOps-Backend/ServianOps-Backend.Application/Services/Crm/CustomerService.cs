using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.DTOs;
using ServianOps_Backend.Application.DTOs.Crm;
using ServianOps_Backend.Application.Interfaces.Crm;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;

namespace ServianOps_Backend.Application.Services.Crm
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CustomerDetailDto> CreateAsync(CreateCustomerDto dto)
        {
            var customer = _mapper.Map<Customer>(dto);

            await _repository.AddAsync(customer);

            var savedCustomer = await _repository.GetQueryable()
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == customer.Id);

            return _mapper.Map<CustomerDetailDto>(savedCustomer);
        }

        public async Task<CustomerDetailDto> UpdateAsync(long id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetQueryable()
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);
                
            if (customer == null) throw new Exception("Customer not found.");

            _mapper.Map(dto, customer);

            var contact = customer.CustomerContacts.FirstOrDefault(c => dto.PrimaryContactId == null || c.Id == dto.PrimaryContactId);
            
            if (contact != null)
            {
                contact.FirstName = dto.ContactFirstName;
                contact.LastName = dto.ContactLastName;
                contact.MobileNumber = dto.ContactMobile;
                contact.Email = dto.ContactEmail;
            }
            else
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

            return _mapper.Map<CustomerDetailDto>(updatedCustomer);
        }

        public async Task<CustomerDetailDto> GetByIdAsync(long id)
        {
            var customer = await _repository.GetQueryable()
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            return customer == null ? null : _mapper.Map<CustomerDetailDto>(customer);
        }

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerListDto>> GetAllPagedAsync(CustomerFilterDto filter)
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
            
            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<CustomerListDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<CustomerListDto>>(customers)
            };
        }

        public async Task<IReadOnlyList<DropdownDto>> GetDropdownAsync()
        {
            var customers = await _repository.GetAllAsync();
            return customers.Select(c => new DropdownDto
            {
                Id = c.Id,
                Name = string.IsNullOrWhiteSpace(c.CompanyName) ? c.Name : c.CompanyName
            }).OrderBy(x => x.Name).ToList();
        }

        public async Task DeleteAsync(long id)
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
            }
        }
    }
}
