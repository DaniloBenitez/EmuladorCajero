using Newtonsoft.Json;
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
        public string fechaApertura { get; set; }
        public string fechaCierre { get; set; }
        public double? saldo { get; set; }
        public double? cuentaId { get; set; }
        public double? titularidadClienteId { get; set; }
        public string numero { get; set; }
        public string descripcion { get; set; }
        public double tipoCuentaId { get; set; }
        public bool esPrincipal { get; set; }
    }
}
