using System;
using System.Collections.Generic;

namespace RETAIL.BASE.OBJ.Models
{
    public class ResultModel<T>
    {

        private List<string> _erros;

        public int StatusCode { get; set; }

        public bool Success { get; set; } = false;

        public T Data { get; set; }

        public string Message { get; set; } = "";

        public DateTime CurrentTime { get; } = DateTime.UtcNow;

        public List<string> Errors
        {
            get
            {
                if (_erros == null)
                {
                    _erros = new List<string>();
                }
                return (_erros);
            }
            set
            {
                _erros = value;
            }
        }
    }
}
