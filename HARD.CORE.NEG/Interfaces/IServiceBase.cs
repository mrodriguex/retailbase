using System.Collections.Generic;
using System.Threading.Tasks;
using HARD.CORE.OBJ.Models;

namespace HARD.CORE.NEG.Interfaces
{
    public interface IServiceBase<MyClassIn, MyClassOut, FilterClass, IdType>
    {
        Task<ResultModel<MyClassOut>> GetByIdAsync(IdType id);
        Task<ResultModel<PagedResult<MyClassOut>>> GetAllAsync(FilterClass filterClass);
        Task<ResultModel<IdType>> AddAsync(MyClassIn entity, int idUsuarioAuenticado);
        Task<ResultModel<bool>> UpdateAsync(MyClassIn entity, int idUsuarioAuenticado);
        Task<ResultModel<bool>> DeleteAsync(IdType id, int idUsuarioAuenticado);
    }
}