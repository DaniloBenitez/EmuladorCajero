using System;
using System.Runtime.Serialization;

namespace EmuladorCajero.DTO
{
    [Serializable]
    public class ResponseTransactionDTO 
    {
        public double id { get; set; }
        public string codigo { get; set; }
        public string numeroToken { get; set; }
        public string fechaVencimientoToken { get; set; }
        public string detalle { get; set; }
        public string fecha { get; set; }
        public double? costoOperacion { get; set; }
        public string terminal { get; set; }
    }
}