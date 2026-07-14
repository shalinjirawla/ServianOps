using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using ServianOps_Backend.Common.Enums;
using ServianOps_Backend.Application.DTOs.Base;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Jobs
{
    public class CreateJobDto
    {
        public long CustomerId { get; set; }
        public long SiteId { get; set; }
        public long TradeId { get; set; }
        public string Description { get; set; }
        public PriorityEnum Priority { get; set; }
        public string PONumber { get; set; }
        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public IFormFileCollection Attachments { get; set; }
    }

    public class UpdateJobDto
    {
        public long CustomerId { get; set; }
        public long SiteId { get; set; }
        public long TradeId { get; set; }
        public string Description { get; set; }
        public PriorityEnum Priority { get; set; }
        public string PONumber { get; set; }
        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public IFormFileCollection Attachments { get; set; }
    }

    public class JobDetailDto : BaseAuditDto
    {
        public string JobNumber { get; set; }
        
        public long CustomerId { get; set; }
        public CustomerSummaryDto Customer { get; set; }

        public long SiteId { get; set; }
        public SiteSummaryDto Site { get; set; }

        public long TradeId { get; set; }
        public TradeDto Trade { get; set; }

        public string Description { get; set; }
        public PriorityEnum Priority { get; set; }
        public string PONumber { get; set; }
        public decimal Budget { get; set; }
        public decimal NTE { get; set; }

        public List<JobAttachmentDto> Attachments { get; set; }
    }

    public class JobListDto : BaseAuditDto
    {
        public string JobNumber { get; set; }
        public CustomerSummaryDto Customer { get; set; }
        public SiteSummaryDto Site { get; set; }
        public TradeDto Trade { get; set; }
        public PriorityEnum Priority { get; set; }
    }
}
