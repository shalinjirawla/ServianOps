using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories;

namespace ServianOps_Backend.Application.SiteModule.Site
{
    public class SiteService : ISiteService
    {
        private readonly IGenericRepository<Core.Entities.Crm.Site> _siteRepository;
        private readonly IMapper _mapper;

        public SiteService(IGenericRepository<Core.Entities.Crm.Site> siteRepository, IMapper mapper)
        {
            _siteRepository = siteRepository;
            _mapper = mapper;
        }

        public async Task<StandardResponse<SiteDetailDto>> CreateSite(CreateSiteDto dto)
        {
            var site = _mapper.Map<Core.Entities.Crm.Site>(dto);

            if (!string.IsNullOrWhiteSpace(dto.ContactFirstName) || 
                !string.IsNullOrWhiteSpace(dto.ContactLastName) || 
                !string.IsNullOrWhiteSpace(dto.ContactEmail) || 
                !string.IsNullOrWhiteSpace(dto.ContactMobile))
            {
                site.SiteContacts = new List<SiteContact>
                {
                    new SiteContact
                    {
                        FirstName = dto.ContactFirstName,
                        LastName = dto.ContactLastName,
                        Email = dto.ContactEmail,
                        MobileNumber = dto.ContactMobile
                    }
                };
            }

            await _siteRepository.AddAsync(site);

            // Fetch created entity to map correctly
            var createdSite = await _siteRepository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == site.Id);

            return StandardResponse<SiteDetailDto>.Ok(_mapper.Map<SiteDetailDto>(createdSite));
        }

        public async Task<StandardResponse<SiteDetailDto>> UpdateSite(long id, UpdateSiteDto dto)
        {
            var site = await _siteRepository.GetQueryable()
                .Include(s => s.SiteContacts)
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (site == null) return StandardResponse<SiteDetailDto>.Error("Site not found.");

            _mapper.Map(dto, site);

            if (dto.PrimaryContactId.HasValue && dto.PrimaryContactId.Value > 0)
            {
                var contact = site.SiteContacts?.FirstOrDefault(c => c.Id == dto.PrimaryContactId.Value);
                if (contact != null)
                {
                    contact.FirstName = dto.ContactFirstName;
                    contact.LastName = dto.ContactLastName;
                    contact.Email = dto.ContactEmail;
                    contact.MobileNumber = dto.ContactMobile;
                }
            }
            else if (!string.IsNullOrWhiteSpace(dto.ContactFirstName))
            {
                site.SiteContacts ??= new List<SiteContact>();
                site.SiteContacts.Add(new SiteContact
                {
                    FirstName = dto.ContactFirstName,
                    LastName = dto.ContactLastName,
                    Email = dto.ContactEmail,
                    MobileNumber = dto.ContactMobile
                });
            }

            await _siteRepository.UpdateAsync(site);

            return StandardResponse<SiteDetailDto>.Ok(_mapper.Map<SiteDetailDto>(site));
        }

        public async Task<StandardResponse<SiteDetailDto>> GetSiteById(long id)
        {
            var site = await _siteRepository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (site == null) return StandardResponse<SiteDetailDto>.Error("Site not found.");

            return StandardResponse<SiteDetailDto>.Ok(_mapper.Map<SiteDetailDto>(site));
        }

        public async Task<StandardResponse<PagedResultDto<SiteListDto>>> GetAllSites(SiteFilterDto filter)
        {
            var query = _siteRepository.GetQueryable()
                .Include(s => s.Customer)
                .Include(s => s.AccountManager)
                .Include(s => s.SiteContacts)
                .AsQueryable();

            if (filter.CustomerId.HasValue)
            {
                query = query.Where(s => s.CustomerId == filter.CustomerId.Value);
            }
            
            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(s => 
                    s.SiteName.Contains(filter.Search) || 
                    s.CompanyName.Contains(filter.Search));
            }

            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var result = new PagedResultDto<SiteListDto>(
                _mapper.Map<IReadOnlyList<SiteListDto>>(items),
                totalCount,
                filter.PageNumber,
                filter.PageSize);

            return StandardResponse<PagedResultDto<SiteListDto>>.Ok(result);
        }

        public async Task<StandardResponse<IReadOnlyList<SiteLookupDto>>> GetSiteLookup(long? customerId)
        {
            var query = _siteRepository.GetQueryable();
            if (customerId.HasValue)
            {
                query = query.Where(s => s.CustomerId == customerId.Value);
            }

            var items = await query.ToListAsync();
            var result = items.Select(s => new SiteLookupDto
            {
                Id = s.Id,
                SiteName = s.SiteName,
                CompanyName = s.CompanyName
            }).OrderBy(x => x.SiteName).ToList();

            return StandardResponse<IReadOnlyList<SiteLookupDto>>.Ok(result);
        }

        public async Task<StandardResponse<bool>> DeleteSite(long id)
        {
            var site = await _siteRepository.GetByIdAsync(id);
            if (site != null)
            {
                await _siteRepository.DeleteAsync(site);
                return StandardResponse<bool>.Ok(true, "Site deleted.");
            }
            return StandardResponse<bool>.Error("Site not found.");
        }
    }
}
