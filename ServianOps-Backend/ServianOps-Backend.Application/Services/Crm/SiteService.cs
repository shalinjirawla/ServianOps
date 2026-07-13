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
    public class SiteService : ISiteService
    {
        private readonly ISiteRepository _repository;
        private readonly IMapper _mapper;

        public SiteService(ISiteRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<SiteDetailDto> CreateAsync(CreateSiteDto dto)
        {
            var site = _mapper.Map<Site>(dto);
            site.SiteContacts = new List<SiteContact>
            {
                new SiteContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    MobileNumber = dto.ContactMobile,
                    Email = dto.ContactEmail,
                    IsActive = true
                }
            };

            await _repository.AddAsync(site);

            var savedSite = await _repository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == site.Id);

            return _mapper.Map<SiteDetailDto>(savedSite);
        }

        public async Task<SiteDetailDto> UpdateAsync(long id, UpdateSiteDto dto)
        {
            var site = await _repository.GetQueryable()
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (site == null) throw new Exception("Site not found.");

            _mapper.Map(dto, site);

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
                    Email = dto.ContactEmail,
                    IsActive = true
                });
            }

            await _repository.UpdateAsync(site);

            var updatedSite = await _repository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == id);

            return _mapper.Map<SiteDetailDto>(updatedSite);
        }

        public async Task<SiteDetailDto> GetByIdAsync(long id)
        {
            var site = await _repository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == id);

            return site == null ? null : _mapper.Map<SiteDetailDto>(site);
        }

        public async Task<IReadOnlyList<SiteListDto>> GetAllPagedAsync(int pageNumber, int pageSize)
        {
            var sites = await _repository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            return _mapper.Map<List<SiteListDto>>(sites);
        }

        public async Task<IReadOnlyList<DropdownDto>> GetSitesByCustomerDropdownAsync(long customerId)
        {
            var sites = await _repository.GetQueryable()
                .Where(s => s.CustomerId == customerId)
                .OrderBy(s => s.SiteName)
                .Select(s => new DropdownDto
                {
                    Id = s.Id,
                    Name = s.SiteName
                })
                .ToListAsync();

            return sites;
        }

        public async Task DeleteAsync(long id)
        {
            var site = await _repository.GetQueryable()
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == id);

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
    }
}
