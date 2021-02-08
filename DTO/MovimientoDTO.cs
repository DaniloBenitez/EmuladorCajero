using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuladorCajero.DTO
{
    public class MovimientoDTO
    {
        public MovimientoDTO()
        {
            transaccion = new TransactionDTO();
        }

        public double id { get; set; }
        public double saldo { get; set; }

        public double importeOrigen { get; set; }

        public TransactionDTO transaccion { get; set; }
    }
}
