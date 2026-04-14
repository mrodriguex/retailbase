using System;

namespace HARD.CORE.OBJ
{
    public class Cliente: Base
    {
        public string RFC { get; set; }
        public string RazonSocial { get; set; }
        public int? IdClientePadre { get; set; }
    }
}
