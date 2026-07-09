using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServianOps_Backend.Core.Entities.Crm;
using ServianOps_Backend.Core.Interfaces.Repositories.Crm;
using ServianOps_Backend.EntityFramework.Contexts;

namespace ServianOps_Backend.EntityFramework.Repositories.Crm
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task<IReadOnlyList<Customer>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts.Where(contact => contact.IsActive))
                .AsNoTracking()
                .OrderByDescending(c => c.CreationTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Customer> GetCustomerWithContactsAsync(long id)
        {
            return await _dbContext.Customers
                .Include(c => c.CustomerType)
                .Include(c => c.AccountManager)
                .Include(c => c.CustomerContacts.Where(contact => contact.IsActive && !contact.IsDeleted))
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // We can override UpdateAsync to explicitly handle the CustomerContact graph if needed,
        // or handle it in the Service using _dbContext directly. The service will fetch GetCustomerWithContactsAsync,
        // modify the object graph, and we can just call UpdateAsync which sets the state.
        // EF Core 'Update' method sets all reachable entities to Modified.
        public override async Task UpdateAsync(Customer entity)
        {
            _dbContext.Customers.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}
