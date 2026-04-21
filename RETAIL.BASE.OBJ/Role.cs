using System;
using System.Collections.Generic;

namespace RETAIL.BASE.OBJ
{
    public class Role : Base
    {

        public List<User> Users { get; set; } = new();       

        public List<MenuItem> MenuItems { get; set; } = new();

    }

}
