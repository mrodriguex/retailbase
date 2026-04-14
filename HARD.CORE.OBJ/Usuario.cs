using System.Collections.Generic;

namespace HARD.CORE.OBJ
{
    public class Usuario : Base
    {

        private string _claveUsuario;
        private string _nombreUsuario;
        private string _apellidoPaterno;
        private string _apellidoMaterno;

        public string ClaveUsuario
        {
            get
            {
                _claveUsuario ??= "";
                return _claveUsuario;
            }
            set
            {
                _claveUsuario = value;
            }
        }
        public int NumeroEmpleado { get; set; }

        public bool IsActive
        {
            get; set;
        }

        public List<Perfil> Perfiles { get; set; } = new();

        public List<Empresa> Empresas { get; set; } = new();

        public string NombreCompleto
        {
            get
            {
                return $"{NombreUsuario} {ApellidoPaterno} {ApellidoMaterno}";
            }
        }
        public string NombreUsuario
        {
            get
            {
                _nombreUsuario ??= "";
                return _nombreUsuario;
            }
            set
            {
                _nombreUsuario = value;
            }
        }

        public string ApellidoPaterno
        {
            get
            {
                _apellidoPaterno ??= "";
                return _apellidoPaterno;
            }
            set
            {
                _apellidoPaterno = value;
            }
        }
        public string ApellidoMaterno
        {
            get
            {
                _apellidoMaterno ??= "";
                return _apellidoMaterno;
            }
            set
            {
                _apellidoMaterno = value;
            }
        }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int NumeroIntentos { get; set; }
        public bool Bloqueado { get; set; }
        public bool CambioContrasena { get; set; }
        public bool Estatus { get; set; }
        public string Fotografia { get; set; }
        public int IdUsuarioPorAusencia { get; set; }
        public string NombreUsuarioPorAusencia { get; set; }

    }

}
