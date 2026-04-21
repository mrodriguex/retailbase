using System.Collections.Generic;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface IServiceBase<MyClassIn, MyClassOut, FilterClass, IdType>
    {
        Task<ResultModel<MyClassOut>> GetByIdAsync(IdType id);
        Task<ResultModel<PagedResult<MyClassOut>>> GetAllAsync(FilterClass filterClass);
        Task<ResultModel<IdType>> AddAsync(MyClassIn entity, int idUserAuenticado);
        Task<ResultModel<bool>> UpdateAsync(MyClassIn entity, int idUserAuenticado);
        Task<ResultModel<bool>> DeleteAsync(IdType id, int idUserAuenticado);
    }
}