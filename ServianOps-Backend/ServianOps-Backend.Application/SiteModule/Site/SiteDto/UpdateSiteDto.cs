namespace ServianOps_Backend.Application.SiteModule.Site.SiteDto
{
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
}
