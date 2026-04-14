using System;
using System.Collections.Generic;

namespace HARD.CORE.OBJ
{
    public class Perfil : Base
    {

        public List<Usuario> Usuarios { get; set; } = new();       

        public List<Menu> Menus { get; set; } = new();

    }

}
