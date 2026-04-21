using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.DAT.Repositories
{
    public class MenuItemDA : IRepositoryBase<MenuItem, BaseFilter, int>
    {

        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<MenuItemDA> _logger;

        public MenuItemDA(RETAIL_BASEDbContext context, ILogger<MenuItemDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<MenuItem> GetByIdAsync(int id)
        {
            var menuitem = await _context.MenuItems.FindAsync(id);
            return menuitem;
        }

        public async Task<PagedResult<MenuItem>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.MenuItems.Where(u =>
                (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || u.Name.Contains(filterClass.Name))
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<MenuItem>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(MenuItem entity)
        {
            _context.MenuItems.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menuitem = await _context.MenuItems.FindAsync(id);
            if (menuitem == null)
            {
                return false;
            }
            _context.MenuItems.Remove(menuitem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(MenuItem entity)
        {
            _context.MenuItems.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
