using System.Threading.Tasks;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface IUserService: IServiceBase<User, User, BaseFilter, int>
    {

        Task<ResultModel<bool>> ExistsAsync(int idUser);

        Task<ResultModel<User>> GetByUsernameAsync(string username);

        Task<ResultModel<bool>> UnlockUserAsync(int idUser, int idUserAutenticado);

        Task<ResultModel<bool>> LockUserAsync(int idUser, int idUserAutenticado);
    }
}