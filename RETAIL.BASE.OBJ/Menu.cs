using System.Collections.Generic;

namespace RETAIL.BASE.OBJ
{
    public class MenuItem : Base
    {
        private int? _menuItemIdFather;
        
        public List<Role> Roles { get; set; } = new();
        
        public string Image { get; set; }
        
        public string Path { get; set; }
        
        public int? MenuItemIdFather { get { return (_menuItemIdFather); } set { _menuItemIdFather = value == 0 ? null : value; } }

    }
}
