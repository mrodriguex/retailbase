using RETAIL.BASE.OBJ;
using RETAIL.BASE.DAT.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class ProductPresentationDA : IRepositoryBase<ProductPresentation, BaseFilter, int>
    {
        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<ProductPresentationDA> _logger;

        public ProductPresentationDA(RETAIL_BASEDbContext context, ILogger<ProductPresentationDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ProductPresentation> GetByIdAsync(int id)
        {
            return await _context.ProductPresentations
                .AsNoTracking()
                .Include(pp => pp.Product)
                .FirstOrDefaultAsync(pp => pp.Id == id);
        }

        public async Task<PagedResult<ProductPresentation>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.ProductPresentations.Where(pp =>
                (!filterClass.Enabled.HasValue || pp.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || pp.Name.Contains(filterClass.Name))
                && (!filterClass.IdMaster.HasValue || pp.ProductId == filterClass.IdMaster.Value)
            );

            var result = await query
                .Include(pp => pp.Product)
                .OrderBy(pp => pp.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<ProductPresentation>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(ProductPresentation entity)
        {
            _context.ProductPresentations.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(ProductPresentation entity)
        {
            var trackedEntity = _context.ProductPresentations.Local.FirstOrDefault(pp => pp.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            _context.ProductPresentations.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var presentation = await _context.ProductPresentations.FirstOrDefaultAsync(pp => pp.Id == id);
            if (presentation != null)
            {
                _context.ProductPresentations.Remove(presentation);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
