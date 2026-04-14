using HARD.CORE.DAT.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HARD.CORE.DAT.Repositories
{
    public class PerfilDA : IRepositoryBase<Perfil, BaseFilter, int>
    {

        private readonly HardCoreDbContext _context;
        private readonly ILogger<PerfilDA> _logger;

        public PerfilDA(HardCoreDbContext context, ILogger<PerfilDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private


        public async Task<Perfil> GetByIdAsync(int id)
        {
            var perfil = await _context.Perfiles
                .Include(p => p.Menus)
                .FirstOrDefaultAsync(p => p.Id == id);
            return perfil;
        }

        public async Task<PagedResult<Perfil>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Perfiles.Where(u => (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Perfil>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Perfil entity)
        {
            foreach (var menu in entity.Menus) { _context.Attach(menu); }
            _context.Perfiles.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Perfil entity)
        {
            foreach (var menu in entity.Menus) { _context.Attach(menu); }
            _context.Perfiles.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var perfil = await _context.Perfiles.FirstOrDefaultAsync(p => p.Id == id);
            if (perfil != null)
            {
                foreach (var menu in perfil.Menus) { _context.Attach(menu); }
                _context.Perfiles.Remove(perfil);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

    }
}
