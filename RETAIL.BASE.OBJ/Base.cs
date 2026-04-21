using System;

namespace RETAIL.BASE.OBJ
{
    public class Base
    {

        private string _description;
        private string _name;
        private string _abbreviation;

        public int Id { get; set; }

        public string Description { get { if (string.IsNullOrEmpty(_description)) { _description = ""; } return (_description); } set { _description = value; } }

        public string Name { get { if (string.IsNullOrEmpty(_name)) { _name = ""; } return (_name); } set { _name = value; } }

        public string Abbreviation { get { if (string.IsNullOrEmpty(_abbreviation)) { _abbreviation = ""; } return (_abbreviation); } set { _abbreviation = value; } }

        public bool Enabled { get; set; } = true;

        public DateTime DateTimeCreation { get; set; } = DateTime.UtcNow;

        public DateTime DateTimeModification { get; set; } = DateTime.UtcNow;

        public int IdUserCreation { get; set; }

        public int IdUserModification { get; set; }

        public int Order { get; set; }

    }
}
