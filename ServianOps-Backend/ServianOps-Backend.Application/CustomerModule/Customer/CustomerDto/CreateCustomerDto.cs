namespace ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto
{
    public class CreateCustomerDto
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

        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactMobile { get; set; }
        public string ContactEmail { get; set; }
    }
}
