using System.Threading.Tasks;
using ServianOps_Backend.Core.Entities.Crm;

namespace ServianOps_Backend.Core.Interfaces.Repositories.Crm
{
    public interface ICustomerTypeRepository : IGenericRepository<CustomerType>
    {
    }

    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer> GetCustomerWithContactsAsync(long id);
    }

    public interface ICustomerContactRepository : IGenericRepository<CustomerContact>
    {
    }

    public interface ISiteRepository : IGenericRepository<Site>
    {
        Task<Site> GetSiteWithContactsAsync(long id);
    }

    public interface ISiteContactRepository : IGenericRepository<SiteContact>
    {
    }
}
