using System.Collections.Generic;
using ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto;
using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.SiteModule.Site.SiteDto
{
    public class SiteDetailDto : BaseAuditDto
    {
        public long CustomerId { get; set; }
        public CustomerLookupDto Customer { get; set; }
        
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

        public List<SiteContactDto> Contacts { get; set; }
    }
}
