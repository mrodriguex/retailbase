using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface IMenuItemService : IServiceBase<MenuItem, MenuItem, BaseFilter, int>
    {
        /// <summary>
        /// Obtains the menuitems associated with a specific profile.
        /// </summary>
        /// <param name="idRole">The unique key identifying the profile.</param>
        /// <returns>A list of menuitems associated with the provided profile key.</returns>
        Task<ResultModel<IEnumerable<MenuItem>>> GetMenuItemsByProfileAsync(int idRole);

        /// <summary>
        /// Obtains the menuitems associated with a specific user.
        /// </summary>
        /// <param name="userName">The unique key identifying the user.</param>
        /// <param name="idRole">The unique key identifying the profile.</param>
        /// <returns>A list of menuitems associated with the provided user and profile keys.</returns>
        Task<ResultModel<IEnumerable<MenuItem>>> GetMenuItemsByUserAsync(int idUser, int idRole);
    }
}
