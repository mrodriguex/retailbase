using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HARD.CORE.NEG.Interfaces
{
    public interface IMenuService : IServiceBase<Menu, Menu, BaseFilter, int>
    {
        /// <summary>
        /// Obtains the menus associated with a specific profile.
        /// </summary>
        /// <param name="idPerfil">The unique key identifying the profile.</param>
        /// <returns>A list of menus associated with the provided profile key.</returns>
        Task<ResultModel<IEnumerable<Menu>>> GetMenusByProfileAsync(int idPerfil);

        /// <summary>
        /// Obtains the menus associated with a specific user.
        /// </summary>
        /// <param name="claveUsuario">The unique key identifying the user.</param>
        /// <param name="idPerfil">The unique key identifying the profile.</param>
        /// <returns>A list of menus associated with the provided user and profile keys.</returns>
        Task<ResultModel<IEnumerable<Menu>>> GetMenusByUserAsync(int idUsuario, int idPerfil);
    }
}
