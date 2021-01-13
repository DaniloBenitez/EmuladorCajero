using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    public enum GrupoToken
    {
        /// <summary>
        /// Remanente
        /// </summary>
        REM,
        /// <summary>
        /// Préstamo
        /// </summary>
        PRE,
        /// <summary>
        /// Transferencia
        /// </summary>
        TRA,
        /// <summary>
        /// Acceso ATM
        /// </summary>
        APP,
        /// <summary>
        /// Registración
        /// </summary>
        REG
    }
}
