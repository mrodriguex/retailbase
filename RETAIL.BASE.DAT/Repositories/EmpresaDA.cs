using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RETAIL.BASE.DAT.Repositories
{
    public class CompanyDA : IRepositoryBase<Company, BaseFilter, int>
    {

        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<CompanyDA> _logger;

        public CompanyDA(RETAIL_BASEDbContext context, ILogger<CompanyDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private


        #endregion

        public async Task<Company> GetByIdAsync(int id)
        {
            var company = await _context.Companys.FirstOrDefaultAsync(e => e.Id == id);
            return company;
        }

        public async Task<PagedResult<Company>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Companys.Where(u => (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Company>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Company entity)
        {
            _context.Companys.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Company entity)
        {
            _context.Companys.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var company = await _context.Companys.FirstOrDefaultAsync(e => e.Id == id);
            if (company == null)
            {
                return false;
            }
            _context.Companys.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
