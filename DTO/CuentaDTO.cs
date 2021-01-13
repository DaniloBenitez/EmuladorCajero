using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    public class CuentaDTO
    {
        public double? id { get; set; }
        public double? idCliente { get; set; }
        public double? codTablaTitularidad { get; set; }
        public string fechaAlta { get; set; }
        public string fechaVigencia { get; set; }
        public double? cuentaId { get; set; }
        public double? titularidadClienteId { get; set; }
        public string numeroCuenta { get; set; }
        public string descripcion { get; set; }
        public string tipoCuenta { get; set; }
        public bool esPrincipal { get; set; }
    }
}
