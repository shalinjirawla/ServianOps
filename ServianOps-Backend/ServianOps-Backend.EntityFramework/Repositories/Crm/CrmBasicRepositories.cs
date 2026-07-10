using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories.Crm
{
    public class CustomerTypeRepository : GenericRepository<CustomerType>, ICustomerTypeRepository
    {
        public CustomerTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class CustomerContactRepository : GenericRepository<CustomerContact>, ICustomerContactRepository
    {
        public CustomerContactRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }

    public class SiteContactRepository : GenericRepository<SiteContact>, ISiteContactRepository
    {
        public SiteContactRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
