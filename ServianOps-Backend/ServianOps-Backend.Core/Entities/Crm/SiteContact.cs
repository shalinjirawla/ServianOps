using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Crm
{
    public class SiteContact : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }

        public long SiteId { get; set; }
        public Site Site { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
    }
}
