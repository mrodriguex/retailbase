using System;
using System.Collections.Generic;
using System.Text;

namespace RETAIL.BASE.OBJ
{
    public class Product : Base
    {
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<ProductPresentation> Presentations { get; set; } = new List<ProductPresentation>();
    }
}