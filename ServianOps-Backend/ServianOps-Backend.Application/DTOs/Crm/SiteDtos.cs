using System;
using System.Collections.Generic;
using ServianOps_Backend.Application.DTOs.Shared;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class SiteContactDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateSiteDto
    {
        public long CustomerId { get; set; }

        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CountryOrState { get; set; }
        public string PostCode { get; set; }
        public string MobileNumber { get; set; }

        public string AccessDetails { get; set; }
        public string ParkingInformation { get; set; }
        public string KeysOrCode { get; set; }
        public string SiteNotes { get; set; }

        public long? AccountManagerId { get; set; }

        // Contact fields (merged into one form)
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
    }

    public class UpdateSiteDto
    {
        public long CustomerId { get; set; }

        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CountryOrState { get; set; }
        public string PostCode { get; set; }
        public string MobileNumber { get; set; }

        public string AccessDetails { get; set; }
        public string ParkingInformation { get; set; }
        public string KeysOrCode { get; set; }
        public string SiteNotes { get; set; }

        public long? AccountManagerId { get; set; }

        public long? PrimaryContactId { get; set; }
        
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
    }

    public class SiteDetailDto
    {
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public CustomerSummaryDto Customer { get; set; }
        
        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CountryOrState { get; set; }
        public string PostCode { get; set; }
        public string MobileNumber { get; set; }

        public string AccessDetails { get; set; }
        public string ParkingInformation { get; set; }
        public string KeysOrCode { get; set; }
        public string SiteNotes { get; set; }

        public long? AccountManagerId { get; set; }
        public string AccountManagerName { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }

        public List<SiteContactDto> Contacts { get; set; }
    }

    public class SiteListDto
    {
        public long Id { get; set; }
        public CustomerSummaryDto Customer { get; set; }
        public string SiteName { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }

        public string AccountManagerName { get; set; }

        public string PrimaryContactName { get; set; }
        public string PrimaryContactMobile { get; set; }

        public DateTime CreationTime { get; set; }
        public bool IsActive { get; set; }
    }
}
