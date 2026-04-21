using System;

namespace RETAIL.BASE.OBJ
{
    public class Customer: Base
    {
        public string TAXID { get; set; }
        public string LegalName { get; set; }
        public int? IdCustomerFather { get; set; }
    }
}
