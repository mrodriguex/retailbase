using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RETAIL.BASE.DAT.Repositories
{
    public class RoleDA : IRepositoryBase<Role, BaseFilter, int>
    {

        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<RoleDA> _logger;

        public RoleDA(RETAIL_BASEDbContext context, ILogger<RoleDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Private


        public async Task<Role> GetByIdAsync(int id)
        {
            var role = await _context.Roles
                .Include(p => p.MenuItems)
                .FirstOrDefaultAsync(p => p.Id == id);
            return role;
        }

        public async Task<PagedResult<Role>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Roles.Where(u => (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Role>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Role entity)
        {
            foreach (var menuitem in entity.MenuItems) { _context.Attach(menuitem); }
            _context.Roles.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(Role entity)
        {
            foreach (var menuitem in entity.MenuItems) { _context.Attach(menuitem); }
            _context.Roles.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(p => p.Id == id);
            if (role != null)
            {
                foreach (var menuitem in role.MenuItems) { _context.Attach(menuitem); }
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        #endregion

    }
}
