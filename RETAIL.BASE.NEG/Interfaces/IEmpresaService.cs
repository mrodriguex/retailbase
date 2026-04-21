using System.Collections.Generic;
using System.Threading.Tasks;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;

namespace RETAIL.BASE.NEG.Interfaces
{
    /// <summary>
    /// Interface for the business logic layer of companies.
    /// </summary>
    public interface ICompanyService : IServiceBase<Company, Company, BaseFilter, int>
    {
        /// <summary>
        /// Obtains a company by its unique key.
        /// </summary>
        /// <param name="claveCompany">The unique key identifying the company.</param>
        /// <returns>The company associated with the provided key.</returns>
        public Task<ResultModel<IEnumerable<Company>>> GetCompaniesByUserAsync(int idUser, int pageIndex = 1, int pageSize = int.MaxValue);

    }
}
