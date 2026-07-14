using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.SiteModule.Site.SiteDto
{
    public class SiteFilterDto : FilterDto
    {
        public long? CustomerId { get; set; }
    }
}
