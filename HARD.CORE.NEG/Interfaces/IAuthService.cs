using System.Collections.Generic;
using System.Threading.Tasks;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;

namespace HARD.CORE.NEG.Interfaces
{
    public interface IAuthService 
    {
               /// <summary>
        /// Authenticates a user based on their unique key and password.
        /// </summary>
        /// <param name="claveUsuario">The unique key identifying the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        Task<ResultModel<bool>> AuthenticateUserAsync(LoginModel login, int idUsuarioAutenticado);

        /// <summary>
        /// Updates the password for a user.
        /// </summary>
        /// <param name="login">The login model containing the user's credentials.</param>
        /// <param name="idUsuarioAutenticado">The ID of the authenticated user performing the update.</param>
        /// <returns><c>true</c> if the password was updated successfully; otherwise, <c>false</c>.</returns>
        Task<ResultModel<bool>> UpdatePasswordAsync(LoginModel login, int idUsuarioAutenticado);
    }
}