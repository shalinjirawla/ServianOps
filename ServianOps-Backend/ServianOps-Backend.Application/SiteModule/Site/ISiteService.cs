using System.Collections.Generic;
using System.Threading.Tasks;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;

namespace ServianOps_Backend.Application.SiteModule.Site
{
    public interface ISiteService
    {
        Task<StandardResponse<SiteDetailDto>> CreateSite(CreateSiteDto dto);
        Task<StandardResponse<SiteDetailDto>> UpdateSite(long id, UpdateSiteDto dto);
        Task<StandardResponse<SiteDetailDto>> GetSiteById(long id);
        Task<StandardResponse<PagedResultDto<SiteListDto>>> GetAllSites(SiteFilterDto filter);
        Task<StandardResponse<IReadOnlyList<SiteLookupDto>>> GetSiteLookup(long? customerId);
        Task<StandardResponse<bool>> DeleteSite(long id);
    }
}
