using System.Collections.Generic;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface IAuthService 
    {
               /// <summary>
        /// Authenticates a user based on their unique key and password.
        /// </summary>
        /// <param name="userName">The unique key identifying the user.</param>
        /// <param name="password">The user's password.</param>
        /// <returns><c>true</c> if the user is authenticated; otherwise, <c>false</c>.</returns>
        Task<ResultModel<bool>> AuthenticateUserAsync(LoginModel login, int idUserAutenticado);

        /// <summary>
        /// Updates the password for a user.
        /// </summary>
        /// <param name="login">The login model containing the user's credentials.</param>
        /// <param name="idUserAutenticado">The ID of the authenticated user performing the update.</param>
        /// <returns><c>true</c> if the password was updated successfully; otherwise, <c>false</c>.</returns>
        Task<ResultModel<bool>> UpdatePasswordAsync(LoginModel login, int idUserAutenticado);
    }
}