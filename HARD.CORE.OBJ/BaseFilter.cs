using System;

namespace HARD.CORE.OBJ
{
    public class BaseFilter
    {

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int? IdMaster { get; set; }

        public int? IdDetail { get; set; }

        public string Descripcion { get; set; }

        public string Nombre { get; set; }

        public string Abreviatura { get; set; }

        public bool? Activo { get; set; }

        public DateTime? FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime? FechaModificacion { get; set; } = DateTime.UtcNow;

        public int? IdUsuarioCreacion { get; set; }

        public int? IdUsuarioModificacion { get; set; }
        public int? Orden { get; set; }

    }
}
