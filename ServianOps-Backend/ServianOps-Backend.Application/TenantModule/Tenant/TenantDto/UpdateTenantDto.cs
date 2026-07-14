using System;

namespace ServianOps_Backend.Application.TenantModule.Tenant.TenantDto
{
    public class UpdateTenantDto
    {
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }
        public long? PlanId { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsTrial { get; set; }
        public bool IsExpired { get; set; }
        public string LogoUrl { get; set; }
        public string TimeZone { get; set; }
        public string Currency { get; set; }
    }
}
