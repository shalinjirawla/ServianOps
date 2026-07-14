using ServianOps_Backend.Application.Common.DTOs;
using ServianOps_Backend.Application.CustomerTypeModule.CustomerType.CustomerTypeDto;

namespace ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto
{
    public class CustomerListDto : BaseAuditDto
    {
        public string Name { get; set; }
        public string CompanyName { get; set; }
        public string MobileNumber { get; set; }

        public CustomerTypeLookupDto CustomerType { get; set; }
        public string AccountManagerName { get; set; }

        public string PrimaryContactName { get; set; }
        public string PrimaryContactMobile { get; set; }
    }
}
