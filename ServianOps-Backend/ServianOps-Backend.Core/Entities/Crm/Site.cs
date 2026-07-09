using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Core.Entities.Crm
{
    public class Site : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public long CustomerId { get; set; }
        public Customer Customer { get; set; }

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
        public User AccountManager { get; set; }

        public ICollection<SiteContact> SiteContacts { get; set; }
    }
}
