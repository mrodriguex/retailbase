using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HARD.CORE.OBJ.Models;
namespace HARD.CORE.DAT.Interfaces
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
