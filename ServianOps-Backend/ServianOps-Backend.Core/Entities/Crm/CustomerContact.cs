using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Crm
{
    public class CustomerContact : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public long CustomerId { get; set; }
        public Customer Customer { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}
