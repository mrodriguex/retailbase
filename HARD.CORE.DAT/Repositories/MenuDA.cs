using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.DAT.Repositories
{
    public class MenuDA : IRepositoryBase<Menu, BaseFilter, int>
    {

        private readonly HardCoreDbContext _context;
        private readonly ILogger<MenuDA> _logger;

        public MenuDA(HardCoreDbContext context, ILogger<MenuDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Menu> GetByIdAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            return menu;
        }

        public async Task<PagedResult<Menu>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Menus.Where(u =>
                (!filterClass.Activo.HasValue || u.Activo == filterClass.Activo.Value)
                && (string.IsNullOrEmpty(filterClass.Nombre) || u.Nombre.Contains(filterClass.Nombre))
            );

            var result = await query
                .OrderBy(e => e.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<Menu>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(Menu entity)
        {
            _context.Menus.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return false;
            }
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAsync(Menu entity)
        {
            _context.Menus.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

    }

}
