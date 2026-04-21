using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface IRoleService : IServiceBase<Role, Role, BaseFilter, int>
    {
        /// <summary>
        /// Obtains the profiles associated with a specific user.
        /// </summary>
        /// <param name="idUser">The unique key identifying the user.</param>
        /// <returns>A list of profiles associated with the specified user.</returns>
        /// <remarks>This method retrieves the user based on the provided unique key and then returns the list of profiles associated with that user. If the user does not exist, it returns an empty list.</remarks>
        /// <exception cref="ArgumentException">Thrown when the provided user key is invalid.</exception>
        Task<ResultModel<IEnumerable<Role>>> GetUserProfilesAsync(int idUser);

        /// <summary>
        /// Assigns a profile to a user.
        /// </summary>
        /// <param name="idUser">The unique key identifying the user.</param>
        /// <param name="idRole">The unique key identifying the profile.</param>
        /// <returns>True if the profile was successfully assigned to the user; otherwise, false.</returns>
        Task<ResultModel<bool>> AssignProfileToUserAsync(int idUser, int idRole);

        /// <summary>
        /// Removes a profile from a user.
        /// </summary>
        /// <param name="idUser">The unique key identifying the user.</param>
        /// <param name="idRole">The unique key identifying the profile.</param>
        /// <returns>True if the profile was successfully removed from the user; otherwise, false.</returns>
        Task<ResultModel<bool>> RemoveProfileFromUserAsync(int idUser, int idRole);

    }
}
