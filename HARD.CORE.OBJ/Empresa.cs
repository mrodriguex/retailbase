using System;
using System.Collections.Generic;
using System.Text;

namespace HARD.CORE.OBJ
{
    public class Empresa : Base
    {
        public List<Usuario> Usuarios { get; set; } = new();

        public string RFC { get; set; }
        public string RazonSocial { get; set; }
    }
}
