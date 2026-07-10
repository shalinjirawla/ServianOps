using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServianOps_Backend.Application.DTOs;
using ServianOps_Backend.Application.DTOs.Crm;
using ServianOps_Backend.Application.DTOs.Shared;
using ServianOps_Backend.Application.Interfaces.Crm;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;

namespace ServianOps_Backend.Application.Services.Crm
{
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _repository;

        public SiteService(ISiteRepository repository)
        {
            _repository = repository;
        }

        public async Task<SiteDetailDto> CreateAsync(CreateSiteDto dto)
        {
            var site = new Site
            {
                CustomerId = dto.CustomerId,
                SiteName = dto.SiteName,
                CompanyName = dto.CompanyName,
                Area = dto.Area,
                City = dto.City,
                CountryOrState = dto.CountryOrState,
                PostCode = dto.PostCode,
                MobileNumber = dto.MobileNumber,
                AccessDetails = dto.AccessDetails,
                ParkingInformation = dto.ParkingInformation,
                KeysOrCode = dto.KeysOrCode,
                SiteNotes = dto.SiteNotes,
                AccountManagerId = dto.AccountManagerId,
                
                SiteContacts = new List<SiteContact>
                {
                    new SiteContact
                    {
                        FirstName = dto.ContactFirstName,
                        LastName = dto.ContactLastName,
                        MobileNumber = dto.ContactMobile,
                        Email = dto.ContactEmail
                    }
                }
            };

            await _repository.AddAsync(site);

            var savedSite = await _repository.GetSiteWithContactsAsync(site.Id);
            return MapToDetailDto(savedSite);
        }

        public async Task<SiteDetailDto> UpdateAsync(long id, UpdateSiteDto dto)
        {
            var site = await _repository.GetSiteWithContactsAsync(id);
            if (site == null) throw new Exception("Site not found.");

            site.CustomerId = dto.CustomerId;
            site.SiteName = dto.SiteName;
            site.CompanyName = dto.CompanyName;
            site.Area = dto.Area;
            site.City = dto.City;
            site.CountryOrState = dto.CountryOrState;
            site.PostCode = dto.PostCode;
            site.MobileNumber = dto.MobileNumber;
            site.AccessDetails = dto.AccessDetails;
            site.ParkingInformation = dto.ParkingInformation;
            site.KeysOrCode = dto.KeysOrCode;
            site.SiteNotes = dto.SiteNotes;
            site.AccountManagerId = dto.AccountManagerId;

            var contact = site.SiteContacts.FirstOrDefault(c => dto.PrimaryContactId == null || c.Id == dto.PrimaryContactId);
            
            if (contact != null)
            {
                contact.FirstName = dto.ContactFirstName;
                contact.LastName = dto.ContactLastName;
                contact.MobileNumber = dto.ContactMobile;
                contact.Email = dto.ContactEmail;
            }
            else
            {
                site.SiteContacts.Add(new SiteContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    MobileNumber = dto.ContactMobile,
                    Email = dto.ContactEmail
                });
            }

            await _repository.UpdateAsync(site);

            var updatedSite = await _repository.GetSiteWithContactsAsync(id);
            return MapToDetailDto(updatedSite);
        }

        public async Task<SiteDetailDto> GetByIdAsync(long id)
        {
            var site = await _repository.GetSiteWithContactsAsync(id);
            return site == null ? null : MapToDetailDto(site);
        }

        public async Task<IReadOnlyList<SiteListDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var sites = await _repository.GetPagedAsync(pageNumber, pageSize);
            
            return sites.Select(s => 
            {
                var primaryContact = s.SiteContacts?.FirstOrDefault();
                return new SiteListDto
                {
                    Id = s.Id,
                    Customer = s.Customer != null ? new CustomerSummaryDto
                    {
                        Id = s.Customer.Id,
                        Name = s.Customer.Name,
                        CompanyName = s.Customer.CompanyName,
                        MobileNumber = s.Customer.MobileNumber
                    } : null,
                    SiteName = s.SiteName,
                    CompanyName = s.CompanyName,
                    MobileNumber = s.MobileNumber,
                    AccountManagerName = s.AccountManager != null ? $"{s.AccountManager.FirstName} {s.AccountManager.LastName}" : null,
                    PrimaryContactName = primaryContact != null ? $"{primaryContact.FirstName} {primaryContact.LastName}" : null,
                    PrimaryContactMobile = primaryContact?.MobileNumber,
                    CreationTime = s.CreationTime,
                    IsActive = s.IsActive
                };
            }).ToList();
        }

        public async Task<IReadOnlyList<DropdownDto>> GetSitesByCustomerDropdownAsync(long customerId)
        {
            var sites = await _repository.GetAllAsync();
            return sites
                .Where(s => s.CustomerId == customerId)
                .Select(s => new DropdownDto
                {
                    Id = s.Id,
                    Name = s.SiteName
                })
                .OrderBy(s => s.Name)
                .ToList();
        }

        public async Task DeleteAsync(long id)
        {
            var site = await _repository.GetSiteWithContactsAsync(id);
            if (site != null)
            {
                foreach (var contact in site.SiteContacts)
                {
                    contact.IsDeleted = true;
                    contact.IsActive = false;
                    contact.DeletedDate = DateTime.UtcNow;
                }
                await _repository.DeleteAsync(site);
            }
        }

        private static SiteDetailDto MapToDetailDto(Site entity)
        {
            return new SiteDetailDto
            {
                Id = entity.Id,
                CustomerId = entity.CustomerId,
                Customer = entity.Customer != null ? new CustomerSummaryDto
                {
                    Id = entity.Customer.Id,
                    Name = entity.Customer.Name,
                    CompanyName = entity.Customer.CompanyName,
                    MobileNumber = entity.Customer.MobileNumber
                } : null,
                SiteName = entity.SiteName,
                CompanyName = entity.CompanyName,
                Area = entity.Area,
                City = entity.City,
                CountryOrState = entity.CountryOrState,
                PostCode = entity.PostCode,
                MobileNumber = entity.MobileNumber,
                AccessDetails = entity.AccessDetails,
                ParkingInformation = entity.ParkingInformation,
                KeysOrCode = entity.KeysOrCode,
                SiteNotes = entity.SiteNotes,
                AccountManagerId = entity.AccountManagerId,
                AccountManagerName = entity.AccountManager != null ? $"{entity.AccountManager.FirstName} {entity.AccountManager.LastName}" : null,
                CreationTime = entity.CreationTime,
                IsActive = entity.IsActive,
                Contacts = entity.SiteContacts?.Select(c => new SiteContactDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    MobileNumber = c.MobileNumber,
                    Email = c.Email,
                    IsActive = c.IsActive
                }).ToList() ?? new List<SiteContactDto>()
            };
        }
    }
}
