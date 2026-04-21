using System.Collections.Generic;

namespace RETAIL.BASE.OBJ.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; } = 0;
        public int PageIndex { get; set; } = 0;
        public int PageSize { get; set; } = 0;
    }
}