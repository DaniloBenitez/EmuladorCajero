using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    class RespuestaTokenDTO
    {
        public RespuestaTokenDTO()
        {
            cuentas = new List<CuentaDTO>();
            tokenCheckDTO = new TokenDTO();
            clienteDTO = new ClienteDTO();
        }
        public string dni { get; set; }
        public bool tieneCuenta { get; set; }
        public List<CuentaDTO> cuentas { get; set; }
        public TokenDTO tokenCheckDTO { get; set; }
        public ClienteDTO clienteDTO { get; set; }
    }
}
