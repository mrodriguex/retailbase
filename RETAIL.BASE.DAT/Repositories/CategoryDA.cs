using RETAIL.BASE.OBJ;
using RETAIL.BASE.DAT.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class CategoryDA : IRepositoryBase<Category, BaseFilter, int>
    {
        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<CategoryDA> _logger;

        public CategoryDA(RETAIL_BASEDbContext context, ILogger<CategoryDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories
                .AsNoTracking()
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PagedResult<Category>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Categories.Where(c =>
                (!filterClass.Enabled.HasValue || c.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || c.Name.Contains(filterClass.Name))
            );

            var result = await query
                .OrderBy(c => c.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Category>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Category entity)
        {
            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Category entity)
        {
            var trackedEntity = _context.Categories.Local.FirstOrDefault(c => c.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
