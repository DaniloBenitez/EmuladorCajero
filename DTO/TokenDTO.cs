using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    class TokenDTO
    {
        public double tokenId { get; set; }
        public string token { get; set; }
        public double idUsuario { get; set; }
        public double? idTipoToken { get; set; }
        public string estadoToken { get; set; }
        public string respuesta { get; set; }
        public bool actualizarEstado { get; set; }
        public double saldoToken { get; set; }
        public bool status { get; set; }
        public string grupoToken { get; set; }
        public GrupoToken Tipo { get { return (GrupoToken)Enum.Parse(typeof(GrupoToken), grupoToken); } }
    }
}
