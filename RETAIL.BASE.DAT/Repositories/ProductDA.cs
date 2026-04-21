using RETAIL.BASE.OBJ;
using RETAIL.BASE.DAT.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class ProductDA : IRepositoryBase<Product, BaseFilter, int>
    {
        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<ProductDA> _logger;

        public ProductDA(RETAIL_BASEDbContext context, ILogger<ProductDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .AsNoTracking()
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Presentations)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PagedResult<Product>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Products.Where(p =>
                (!filterClass.Enabled.HasValue || p.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || p.Name.Contains(filterClass.Name))
            );

            var result = await query
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .OrderBy(p => p.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Product entity)
        {
            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Product entity)
        {
            var trackedEntity = _context.Products.Local.FirstOrDefault(p => p.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
