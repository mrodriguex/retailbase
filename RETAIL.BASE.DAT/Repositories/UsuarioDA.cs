
using RETAIL.BASE.OBJ;
using System.Data;

using RETAIL.BASE.DAT.Interfaces;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.DAT.Repositories
{
    public class UserDA : IRepositoryBase<User, BaseFilter, int>
    {

        private readonly RETAIL_BASEDbContext _context;
        private readonly ILogger<UserDA> _logger;

        public UserDA(RETAIL_BASEDbContext context, ILogger<UserDA> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Public

        public async Task<User> GetByIdAsync(int id)
        {
            var user = await _context.Users
                .AsNoTracking()
                .Include(u => u.Companys)
                .Include(u => u.Roles)
                .ThenInclude(p => p.MenuItems)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedResult<User>> GetAllAsync(BaseFilter filterClass)
        {
            var query = _context.Users.Where(u => (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (!filterClass.Enabled.HasValue || u.Enabled == filterClass.Enabled.Value)
                && (string.IsNullOrEmpty(filterClass.Name) || u.UserName.Contains(filterClass.Name))
            );

            var result = await query
                .OrderBy(u => u.Id)
                .Skip((filterClass.PageIndex - 1) * filterClass.PageSize)
                .Take(filterClass.PageSize)
                .ToListAsync();

            return new PagedResult<User>
            {
                Data = result,
                PageIndex = filterClass.PageIndex,
                PageSize = filterClass.PageSize,
                TotalCount = await query.CountAsync()
            };
        }

        public async Task<int> AddAsync(User entity)
        {
            foreach (var role in entity.Roles) { _context.Attach(role); }
            foreach (var company in entity.Companys) { _context.Attach(company); }
            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(User entity)
        {
            var trackedEntity = _context.Users.Local.FirstOrDefault(u => u.Id == entity.Id);
            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity).State = EntityState.Detached;
            }

            foreach (var role in entity.Roles) { _context.Attach(role); }
            foreach (var company in entity.Companys) { _context.Attach(company); }
            _context.Users.Update(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                foreach (var role in user.Roles) { _context.Attach(role); }
                foreach (var company in user.Companys) { _context.Attach(company); }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        #endregion
    }

}