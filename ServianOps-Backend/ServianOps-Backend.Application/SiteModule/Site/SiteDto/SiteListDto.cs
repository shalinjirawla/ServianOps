using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.SiteModule.Site.SiteDto
{
    public class SiteListDto : BaseAuditDto
    {
        public CustomerLookupDto Customer { get; set; }
        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }

        public string AccountManagerName { get; set; }

        public string PrimaryContactName { get; set; }
        public string PrimaryContactMobile { get; set; }
    }
}
