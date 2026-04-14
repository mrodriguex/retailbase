using System.Threading.Tasks;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;

namespace HARD.CORE.NEG.Interfaces
{
    public interface IUsuarioService: IServiceBase<Usuario, Usuario, BaseFilter, int>
    {

        Task<ResultModel<bool>> ExistsAsync(int idUsuario);

        Task<ResultModel<Usuario>> GetByUsernameAsync(string username);

        Task<ResultModel<bool>> UnlockUserAsync(int idUsuario, int idUsuarioAutenticado);

        Task<ResultModel<bool>> LockUserAsync(int idUsuario, int idUsuarioAutenticado);
    }
}