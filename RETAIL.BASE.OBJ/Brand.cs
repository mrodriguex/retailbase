using System;
using System.Collections.Generic;
using System.Text;

namespace RETAIL.BASE.OBJ
{
    public class Brand : Base
    {
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}