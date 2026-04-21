using System.Collections.Generic;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    public interface ICustomerService : IServiceBase<Customer, Customer, BaseFilter, int>
    {
        
    }
}