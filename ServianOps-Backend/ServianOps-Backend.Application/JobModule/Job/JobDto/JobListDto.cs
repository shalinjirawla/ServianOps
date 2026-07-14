using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;
using ServianOps_Backend.Common.Enums;

namespace ServianOps_Backend.Application.JobModule.Job.JobDto
{
    public class JobListDto : BaseAuditDto
    {
        public string JobNumber { get; set; }
        public CustomerLookupDto Customer { get; set; }
        public SiteLookupDto Site { get; set; }
        public TradeLookupDto Trade { get; set; }
        public PriorityEnum Priority { get; set; }
    }
}
