using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;
using ServianOps_Backend.Core.Entities.Identity;

namespace ServianOps_Backend.Core.Entities.Crm
{
    public class Customer : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string Area { get; set; }
        public string City { get; set; }
        public string CountryOrState { get; set; }
        public string PostCode { get; set; }
        public string MobileNumber { get; set; }
        public string AccountNumber { get; set; }

        public int PaymentTerms { get; set; }
        public bool IsVatRegistered { get; set; }
        public string VatNumber { get; set; }
        public bool IsPORequired { get; set; }

        public long CustomerTypeId { get; set; }
        public CustomerType CustomerType { get; set; }

        public long? AccountManagerId { get; set; }
        public User AccountManager { get; set; }

        public long? SellingRateId { get; set; }

        public ICollection<CustomerContact> CustomerContacts { get; set; }
        public ICollection<Site> Sites { get; set; }
    }
}
