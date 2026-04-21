using System;

namespace RETAIL.BASE.OBJ
{
    public class BaseFilter
    {

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? IdMaster { get; set; }

        public int? IdDetail { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }

        public bool? Enabled { get; set; }

        public DateTime? DateTimeCreation { get; set; } = DateTime.UtcNow;

        public DateTime? DateTimeModification { get; set; } = DateTime.UtcNow;

        public int? IdUserCreation { get; set; }

        public int? IdUserModification { get; set; }
        public int? Order { get; set; }

    }
}
