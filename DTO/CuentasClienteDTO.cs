using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    public class CuentasClienteDTO
    {
        public string nombre { get; set; }
        public string apellido { get; set; }
        public List<CuentaDTO> cuentas { get; set; }
    }
}
