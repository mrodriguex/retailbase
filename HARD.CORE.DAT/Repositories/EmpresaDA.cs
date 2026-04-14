using HARD.CORE.DAT.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HARD.CORE.DAT.Repositories
{
    public class EmpresaDA : IRepositoryBase<Empresa, BaseFilter, int>
    {

        private readonly HardCoreDbContext _context;
        private readonly ILogger<EmpresaDA> _logger;

        public EmpresaDA(HardCoreDbContext context, ILogger<EmpresaDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private


        #endregion

        public async Task<Empresa> GetByIdAsync(int id)
        {
            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == id);
            return empresa;
        }

        public async Task<PagedResult<Empresa>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Empresas.Where(u => (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Empresa>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Empresa entity)
        {
            _context.Empresas.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Empresa entity)
        {
            _context.Empresas.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var empresa = await _context.Empresas.FirstOrDefaultAsync(e => e.Id == id);
            if (empresa == null)
            {
                return false;
            }
            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
