using System.Collections.Generic;

namespace RETAIL.BASE.OBJ
{
    public class User : Base
    {

        private string _userName;
        private string _firstName;
        private string _lastNameFather;
        private string _lastNameMother;

        public string UserName
        {
            get
            {
                _userName ??= "";
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        public int EmployeeId { get; set; }

        public List<Role> Roles { get; set; } = new();

        public List<Company> Companys { get; set; } = new();

        public string FirstName
        {
            get
            {
                _firstName ??= "";
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        public string LastNameFather
        {
            get
            {
                _lastNameFather ??= "";
                return _lastNameFather;
            }
            set
            {
                _lastNameFather = value;
            }
        }
        public string LastNameMother
        {
            get
            {
                _lastNameMother ??= "";
                return _lastNameMother;
            }
            set
            {
                _lastNameMother = value;
            }
        }
        public string Email { get; set; }
        public string Password { get; set; }
        public int Attempts { get; set; }
        public string Avatar { get; set; }

    }

}
