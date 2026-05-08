using System.Threading.Tasks;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.OBJ.Ports
{
    public interface IRepositoryBase<MyClass, FilterClass, IdType>
    {
        Task<MyClass> GetByIdAsync(IdType id);
        Task<PagedResult<MyClass>> GetAllAsync(FilterClass filterClass);
        Task<IdType> AddAsync(MyClass entity);
        Task<bool> UpdateAsync(MyClass entity);
        Task<bool> DeleteAsync(IdType id);
    }
}
