using System;

namespace ServianOps_Backend.Application.DTOs.Tenant
{
    public class CreateTenantDto
    {
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public long? PlanId { get; set; }
        public string Password { get; set; } // Needed for initial Admin user
    }
}
