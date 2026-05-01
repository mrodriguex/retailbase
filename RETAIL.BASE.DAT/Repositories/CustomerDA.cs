using RETAIL.BASE.OBJ;
using System.Data;
using System.Linq;

using RETAIL.BASE.DAT.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class CustomerDA : IRepositoryBase<Customer, BaseFilter, int>
    {

        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<CustomerDA> _logger;

        public CustomerDA(RETAIL_BASEDbContext context, ILogger<CustomerDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Public  

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            return customer;
        }

        public async Task<PagedResult<Customer>> GetAllAsync(BaseFilter filterClass)
        {          
            var query = _context.Customers.Where(u => (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
            );
         
            var result = await query
                .OrderBy(c => c.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };

        }

        public async Task<int> AddAsync(Customer entity)
        {
            _context.Customers.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Customer entity)
        {
            _context.Customers.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
            if (customer == null)
            {
                return false;
            }
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

    }
}