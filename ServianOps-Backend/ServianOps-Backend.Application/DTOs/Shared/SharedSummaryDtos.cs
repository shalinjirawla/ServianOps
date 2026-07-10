using System;

namespace ServianOps_Backend.Application.DTOs.Shared
{
    public class CustomerSummaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }
    }

    public class SiteSummaryDto
    {
        public long Id { get; set; }
        public string SiteName { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
    }

    public class UserSummaryDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }

    public class TenantSummaryDto
    {
        public long Id { get; set; }
        public string CompanyName { get; set; }
        public string TenancyName { get; set; }
    }

    public class PlanSummaryDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
