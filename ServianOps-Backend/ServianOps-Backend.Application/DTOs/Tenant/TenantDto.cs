using System;
using System.Collections.Generic;
using ServianOps_Backend.Application.DTOs.Plan;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Tenant
{
    public class TenantDto
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }

        public long? PlanId { get; set; }
        public PlanDto Plan { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsTrial { get; set; }
        public bool IsExpired { get; set; }
        public string LogoUrl { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreationTime { get; set; }
        
        public List<UserSummaryDto> Users { get; set; }
    }
}
