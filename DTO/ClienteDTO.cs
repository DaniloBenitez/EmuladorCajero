using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    public class ClienteDTO
    {
        public double id { get; set; }
        public string dni { get; set; }
        public double? idUsuario { get; set; }
        public string alias { get; set; }
        public string fechaRegistro { get; set; }
        public string fechaBaja { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string origenRegistro { get; set; }
        public string numeroTelefono { get; set; }
    }
}
