using RETAIL.BASE.OBJ;
using RETAIL.BASE.DAT.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class BrandDA : IRepositoryBase<Brand, BaseFilter, int>
    {
        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<BrandDA> _logger;

        public BrandDA(RETAIL_BASEDbContext context, ILogger<BrandDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands
                .AsNoTracking()
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<PagedResult<Brand>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Brands.Where(b =>
                (!filterClass.Enabled.HasValue || b.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || b.Name.Contains(filterClass.Name))
            );

            var result = await query
                .OrderBy(b => b.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Brand>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Brand entity)
        {
            _context.Brands.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Brand entity)
        {
            var trackedEntity = _context.Brands.Local.FirstOrDefault(b => b.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.Brands.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
            if (brand != null)
            {
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
