using System;

namespace HARD.CORE.OBJ
{
    public class Base
    {

        private string _descripcion;
        private string _nombre;
        private string _abreviatura;

        public int Id { get; set; }

        public string Descripcion { get { if (string.IsNullOrEmpty(_descripcion)) { _descripcion = ""; } return (_descripcion); } set { _descripcion = value; } }

        public string Nombre { get { if (string.IsNullOrEmpty(_nombre)) { _nombre = ""; } return (_nombre); } set { _nombre = value; } }

        public string Abreviatura { get { if (string.IsNullOrEmpty(_abreviatura)) { _abreviatura = ""; } return (_abreviatura); } set { _abreviatura = value; } }

        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public DateTime FechaModificacion { get; set; } = DateTime.UtcNow;

        public int IdUsuarioCreacion { get; set; }

        public int IdUsuarioModificacion { get; set; }

        public int Orden { get; set; }

    }
}
