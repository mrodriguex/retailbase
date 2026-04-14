using HARD.CORE.OBJ;
using System.Data;
using System.Linq;

using HARD.CORE.DAT.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HARD.CORE.OBJ.Models;

namespace HARD.CORE.DAT.Repositories
{
    public class ClienteDA : IRepositoryBase<Cliente, BaseFilter, int>
    {

        private readonly HardCoreDbContext _context;
        private readonly ILogger<ClienteDA> _logger;

        public ClienteDA(HardCoreDbContext context, ILogger<ClienteDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Public  

        public async Task<Cliente> GetByIdAsync(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            return cliente;
        }

        public async Task<PagedResult<Cliente>> GetAllAsync(BaseFilter filterClass)
        {          
            var query = _context.Clientes.Where(u => (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
            );
         
            var result = await query
                .OrderBy(c => c.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Cliente>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };

        }

        public async Task<int> AddAsync(Cliente entity)
        {
            _context.Clientes.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Cliente entity)
        {
            _context.Clientes.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Id == id);
            if (cliente == null)
            {
                return false;
            }
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

    }
}