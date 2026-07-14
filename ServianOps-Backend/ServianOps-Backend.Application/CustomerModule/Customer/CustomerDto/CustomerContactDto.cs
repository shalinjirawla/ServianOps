using ServianOps_Backend.Application.Common.DTOs;

namespace ServianOps_Backend.Application.CustomerModule.Customer.CustomerDto
{
    public class CustomerContactDto : BaseAuditDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}
