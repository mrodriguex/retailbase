
using HARD.CORE.OBJ;
using System.Data;

using HARD.CORE.DAT.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HARD.CORE.OBJ.Models;

namespace HARD.CORE.DAT.Repositories
{
    public class UsuarioDA : IRepositoryBase<Usuario, BaseFilter, int>
    {

        private readonly HardCoreDbContext _context;
        private readonly ILogger<UsuarioDA> _logger;

        public UsuarioDA(HardCoreDbContext context, ILogger<UsuarioDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Public

        public async Task<Usuario> GetByIdAsync(int id)
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .Include(u => u.Empresas)
                .Include(u => u.Perfiles)
                .ThenInclude(p => p.Menus)
                .FirstOrDefaultAsync(u => u.Id == id);
            return usuario;
        }

        public async Task<PagedResult<Usuario>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Usuarios.Where(u => (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (string.IsNullOrEmpty(filterClass.Nombre) || u.ClaveUsuario.Contains(filterClass.Nombre))
            );

            var result = await query
                .OrderBy(u => u.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Usuario>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Usuario entity)
        {
            foreach (var perfil in entity.Perfiles) { _context.Attach(perfil); }
            foreach (var empresa in entity.Empresas) { _context.Attach(empresa); }
            _context.Usuarios.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Usuario entity)
        {
            var trackedEntity = _context.Usuarios.Local.FirstOrDefault(u => u.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            foreach (var perfil in entity.Perfiles) { _context.Attach(perfil); }
            foreach (var empresa in entity.Empresas) { _context.Attach(empresa); }
            _context.Usuarios.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
            if (usuario != null)
            {
                foreach (var perfil in usuario.Perfiles) { _context.Attach(perfil); }
                foreach (var empresa in usuario.Empresas) { _context.Attach(empresa); }
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion
    }

}