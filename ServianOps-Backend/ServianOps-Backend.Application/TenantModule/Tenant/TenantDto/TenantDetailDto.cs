using System;
using System.Collections.Generic;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;

namespace ServianOps_Backend.Application.TenantModule.Tenant.TenantDto
{
    public class TenantDetailDto : BaseAuditDto
    {
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }

        public long? PlanId { get; set; }
        public PlanLookupDto Plan { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsTrial { get; set; }
        public bool IsExpired { get; set; }
        public string LogoUrl { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
        
        // We probably don't need Users list in TenantDetailDto directly or we could map them to UserLookupDto
        // public List<UserLookupDto> Users { get; set; }
    }
}
