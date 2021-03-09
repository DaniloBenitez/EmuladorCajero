using System;
using System.Runtime.Serialization;

namespace EmuladorCajero.DTO
{
    public class TransactionDTO 
    {
        public string dniDestino { get; set; }
        public string dniOrigen { get; set; }
        public string fecha { get; set; }
        public decimal importe { get; set; }
        public string moneda { get; set; }
        public string numTarjeta { get; set; }
        public string origenSolicitud { get; set; }
        public string terminal { get; set; }
        public string tipoTarjeta { get; set; }
        public string trace { get; set; }
        public string numeroCuenta { get; set; }
        public double? tokenId { get; set; }
        public string origenRegistro { get; set; }
        public double idUsuario { get; set; }

        public double comision { get; set; }

        public double costoOperacion { get; set; }
    }
}
