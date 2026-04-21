using System;
using System.Collections.Generic;
using System.Text;

namespace RETAIL.BASE.OBJ
{
    public class Company : Base
    {
        public List<User> Users { get; set; } = new();

        public string TAXID { get; set; }
        public string LegalName { get; set; }
    }
}
