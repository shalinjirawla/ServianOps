using System;
using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Saas
{
    public class Plan : BaseEntity
    {
        public string PlanName { get; set; }
        public int MaxUsers { get; set; }
        public int MaxProjects { get; set; }
        public int MaxStorageGB { get; set; }
        public decimal Price { get; set; }
        public string BillingCycle { get; set; } // Monthly/Yearly
        public bool IsTrialAvailable { get; set; }
        public int TrialDays { get; set; }

        public ICollection<Tenant> Tenants { get; set; }
    }
}
