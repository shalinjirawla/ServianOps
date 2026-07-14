using System;
using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.PlanModule.Plan.PlanDto;

namespace ServianOps_Backend.Application.TenantModule.Tenant.TenantDto
{
    public class TenantListDto : BaseAuditDto
    {
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }

        public PlanLookupDto Plan { get; set; }

        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
        public bool IsTrial { get; set; }
        public bool IsExpired { get; set; }

        public ServianOps_Backend.Application.UserModule.User.UserDto.UserLookupDto AdminUser { get; set; }
    }
}
