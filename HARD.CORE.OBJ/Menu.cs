using System.Collections.Generic;

namespace HARD.CORE.OBJ
{
    public class Menu : Base
    {
        private int? _claveMenuPadre;
        
        public List<Perfil> Perfiles { get; set; } = new();
        
        public string Imagen { get; set; }
        
        public string Ruta { get; set; }
        
        public int? ClaveMenuPadre { get { return (_claveMenuPadre); } set { _claveMenuPadre = value == 0 ? null : value; } }

    }
}
