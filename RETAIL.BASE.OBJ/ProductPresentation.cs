using System;
using System.Collections.Generic;
using System.Text;

namespace RETAIL.BASE.OBJ
{
    public class ProductPresentation : Base
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string Barcode { get; set; } = null!;
        public string SizeLabel { get; set; } = null!;

        public decimal NetContent { get; set; }
        public string Unit { get; set; } = null!;

        public string Presentation { get; set; } = null!;

        public decimal? SuggestedPrice { get; set; }
        public decimal? CostEstimate { get; set; }
    }
}