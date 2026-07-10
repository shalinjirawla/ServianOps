using System.Collections.Generic;
using ServianOps_Backend.Core.Entities.Base;

namespace ServianOps_Backend.Core.Entities.Crm
{
    public class CustomerType : BaseEntity, IMustHaveTenant
    {
        public long TenantId { get; set; }
        
        public string Name { get; set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
