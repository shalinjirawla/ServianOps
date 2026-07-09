using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs.Crm;
using ServianOps_Backend.Application.Interfaces.Crm;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;

namespace ServianOps_Backend.Application.Services.Crm
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerDetailDto> CreateAsync(CreateCustomerDto dto)
        {
            var customer = new Customer
            {
                Name = dto.Name,
                CompanyName = dto.CompanyName,
                Area = dto.Area,
                City = dto.City,
                CountryOrState = dto.CountryOrState,
                PostCode = dto.PostCode,
                MobileNumber = dto.MobileNumber,
                AccountNumber = dto.AccountNumber,
                PaymentTerms = dto.PaymentTerms,
                IsVatRegistered = dto.IsVatRegistered,
                VatNumber = dto.VatNumber,
                IsPORequired = dto.IsPORequired,
                CustomerTypeId = dto.CustomerTypeId,
                AccountManagerId = dto.AccountManagerId,
                SellingRateId = dto.SellingRateId,
                
                // Construct the object graph to insert both in one transaction
                CustomerContacts = new List<CustomerContact>
                {
                    new CustomerContact
                    {
                        FirstName = dto.ContactFirstName,
                        LastName = dto.ContactLastName,
                        MobileNumber = dto.ContactMobile,
                        Email = dto.ContactEmail
                    }
                }
            };

            await _repository.AddAsync(customer);

            // Fetch fully populated entity with includes
            var savedCustomer = await _repository.GetCustomerWithContactsAsync(customer.Id);
            return MapToDetailDto(savedCustomer);
        }

        public async Task<CustomerDetailDto> UpdateAsync(long id, UpdateCustomerDto dto)
        {
            var customer = await _repository.GetCustomerWithContactsAsync(id);
            if (customer == null) throw new Exception("Customer not found.");

            // Update parent properties
            customer.Name = dto.Name;
            customer.CompanyName = dto.CompanyName;
            customer.Area = dto.Area;
            customer.City = dto.City;
            customer.CountryOrState = dto.CountryOrState;
            customer.PostCode = dto.PostCode;
            customer.MobileNumber = dto.MobileNumber;
            customer.AccountNumber = dto.AccountNumber;
            customer.PaymentTerms = dto.PaymentTerms;
            customer.IsVatRegistered = dto.IsVatRegistered;
            customer.VatNumber = dto.VatNumber;
            customer.IsPORequired = dto.IsPORequired;
            customer.CustomerTypeId = dto.CustomerTypeId;
            customer.AccountManagerId = dto.AccountManagerId;
            customer.SellingRateId = dto.SellingRateId;

            // Update primary contact
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
                // If no contact exists (unlikely if strictly followed), add a new one to the graph
                customer.CustomerContacts.Add(new CustomerContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    MobileNumber = dto.ContactMobile,
                    Email = dto.ContactEmail
                });
            }

            // Using UpdateAsync effectively marks the root and reachable modified entities to be saved in one transaction
            await _repository.UpdateAsync(customer);

            // Fetch to ensure we return fresh data with related entities
            var updatedCustomer = await _repository.GetCustomerWithContactsAsync(id);
            return MapToDetailDto(updatedCustomer);
        }

        public async Task<CustomerDetailDto> GetByIdAsync(long id)
        {
            var customer = await _repository.GetCustomerWithContactsAsync(id);
            return customer == null ? null : MapToDetailDto(customer);
        }

        public async Task<IReadOnlyList<CustomerListDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var customers = await _repository.GetPagedAsync(pageNumber, pageSize);
            
            return customers.Select(c => 
            {
                var primaryContact = c.CustomerContacts?.FirstOrDefault();
                return new CustomerListDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyName = c.CompanyName,
                    MobileNumber = c.MobileNumber,
                    CustomerTypeName = c.CustomerType?.Name,
                    AccountManagerName = c.AccountManager != null ? $"{c.AccountManager.FirstName} {c.AccountManager.LastName}" : null,
                    PrimaryContactName = primaryContact != null ? $"{primaryContact.FirstName} {primaryContact.LastName}" : null,
                    PrimaryContactMobile = primaryContact?.MobileNumber,
                    CreationTime = c.CreationTime,
                    IsActive = c.IsActive
                };
            }).ToList();
        }

        public async Task DeleteAsync(long id)
        {
            // We fetch the full graph so soft delete marks the parent and ideally the children
            // GenericRepository DeleteAsync removes the root entity. The DbContext overrides SaveChangesAsync 
            // and sets IsDeleted = true. This only affects the explicitly deleted entity.
            // If we want cascading soft delete, we must manually delete the children or configure it.
            var customer = await _repository.GetCustomerWithContactsAsync(id);
            if (customer != null)
            {
                // Since EF Core does not cascade soft deletes automatically via ChangeTracker unless explicitly deleted:
                foreach (var contact in customer.CustomerContacts)
                {
                    contact.IsDeleted = true;
                    contact.IsActive = false;
                    contact.DeletedDate = DateTime.UtcNow;
                    // Note: DeletedBy is tricky without _currentUser in service. Better to use DbContext delete or just let it be.
                    // Actually, calling Remove on the parent does not remove children if DeleteBehavior is Restrict.
                    // To trigger soft delete on children, we can just do:
                    // _dbContext.Remove(contact); but we are in Service.
                    // Let's keep it simple: We just soft delete the parent. When fetching contacts, we can filter out if parent is deleted,
                    // but since IMustHaveTenant and IAuditEntity are global, fetching contacts directly later might return them if they aren't deleted.
                }

                // If we want to physically trigger soft delete logic in SaveChanges, we can use a separate Repo or we just DeleteAsync the customer.
                await _repository.DeleteAsync(customer);
            }
        }

        private static CustomerDetailDto MapToDetailDto(Customer entity)
        {
            return new CustomerDetailDto
            {
                Id = entity.Id,
                Name = entity.Name,
                CompanyName = entity.CompanyName,
                Area = entity.Area,
                City = entity.City,
                CountryOrState = entity.CountryOrState,
                PostCode = entity.PostCode,
                MobileNumber = entity.MobileNumber,
                AccountNumber = entity.AccountNumber,
                PaymentTerms = entity.PaymentTerms,
                IsVatRegistered = entity.IsVatRegistered,
                VatNumber = entity.VatNumber,
                IsPORequired = entity.IsPORequired,
                CustomerTypeId = entity.CustomerTypeId,
                CustomerTypeName = entity.CustomerType?.Name,
                AccountManagerId = entity.AccountManagerId,
                AccountManagerName = entity.AccountManager != null ? $"{entity.AccountManager.FirstName} {entity.AccountManager.LastName}" : null,
                SellingRateId = entity.SellingRateId,
                CreationTime = entity.CreationTime,
                IsActive = entity.IsActive,
                Contacts = entity.CustomerContacts?.Select(c => new CustomerContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    MobileNumber = c.MobileNumber,
                    Email = c.Email,
                    IsActive = c.IsActive
                }).ToList() ?? new List<CustomerContactDto>()
            };
        }
    }
}
