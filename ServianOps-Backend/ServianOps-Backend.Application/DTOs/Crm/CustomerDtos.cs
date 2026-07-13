using System;
using System.Collections.Generic;
using ServianOps_Backend.Application.DTOs.Base;

namespace ServianOps_Backend.Application.DTOs.Crm
{
    public class CustomerContactDto : BaseAuditDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }

    public class CreateCustomerDto
    {
        // Customer fields
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
        public long? AccountManagerId { get; set; }
        public long? SellingRateId { get; set; }

        // Contact fields (merged into one form)
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
    }

    public class UpdateCustomerDto
    {
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
        public long? AccountManagerId { get; set; }
        public long? SellingRateId { get; set; }

        // Optionally, if the frontend sends the primary contact ID:
        public long? PrimaryContactId { get; set; }
        
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
    }

    public class CustomerDetailDto : BaseAuditDto
    {
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
        public CustomerTypeDto CustomerType { get; set; }
        
        public long? AccountManagerId { get; set; }
        public string AccountManagerName { get; set; }

        public long? SellingRateId { get; set; }

        public List<CustomerContactDto> Contacts { get; set; }
    }

    public class CustomerListDto : BaseAuditDto
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }

        public CustomerTypeDto CustomerType { get; set; }
        public string AccountManagerName { get; set; }

        // Primary Contact details mapped from the first active contact
        public string PrimaryContactName { get; set; }
        public string PrimaryContactMobile { get; set; }
    }
}
