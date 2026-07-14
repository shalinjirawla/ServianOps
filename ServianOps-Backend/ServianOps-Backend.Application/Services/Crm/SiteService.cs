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

        public async Task<ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<SiteListDto>> GetAllPagedAsync(SiteFilterDto filter)
        {
            var query = _repository.GetQueryable();

            if (filter.IsActive.HasValue)
            {
                // Active filter if applicable
            }

            if (filter.CustomerId.HasValue)
            {
                query = query.Where(s => s.CustomerId == filter.CustomerId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(s => s.SiteName.Contains(filter.Search) || s.City.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var sites = await query
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            
            return new ServianOps_Backend.Application.DTOs.Shared.PagedResponseDto<SiteListDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = _mapper.Map<List<SiteListDto>>(sites)
            };
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
