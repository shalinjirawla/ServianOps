using System;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Saas
{
    public class Tenant : BaseEntity
    {
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }

        
        public long? PlanId { get; set; }
        public Plan? Plan { get; set; }
        
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsTrial { get; set; }
        public bool IsExpired { get; set; }
        
        public string LogoUrl { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
    }
}
