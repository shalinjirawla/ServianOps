using System;
using ServianOps_Backend.Application.DTOs.Base;

namespace ServianOps_Backend.Application.DTOs.Plan
{
    public class PlanDto : BaseAuditDto
    {
        public string PlanName { get; set; }
        public int MaxUsers { get; set; }
        public int MaxProjects { get; set; }
        public int MaxStorageGB { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; }
        public bool IsTrialAvailable { get; set; }
        public int TrialDays { get; set; }
    }
}
