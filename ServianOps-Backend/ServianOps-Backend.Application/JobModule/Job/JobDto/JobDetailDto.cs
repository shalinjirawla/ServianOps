using System.Collections.Generic;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.SiteModule.Site.SiteDto;
using ServianOps_Backend.Application.TradeModule.Trade.TradeDto;
using ServianOps_Backend.Common.Enums;

namespace ServianOps_Backend.Application.JobModule.Job.JobDto
{
    public class JobDetailDto : BaseAuditDto
    {
        public string JobNumber { get; set; }
        
        public long CustomerId { get; set; }
        public CustomerLookupDto Customer { get; set; }

        public long SiteId { get; set; }
        public SiteLookupDto Site { get; set; }

        public long TradeId { get; set; }
        public TradeLookupDto Trade { get; set; }

        public string Description { get; set; }
        public PriorityEnum Priority { get; set; }
        public string PONumber { get; set; }
        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public List<JobAttachmentDto> Attachments { get; set; }
    }
}
