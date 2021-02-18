using EmuladorCajero.DTO;
using System;

namespace EmuladorCajero
{
    class Program
    {
        static void Main(string[] args)
        {
            string salida = string.Empty;
            long terminalID = int.Parse(eCatConfig.GetValue("TerminalID"));
            ResponseDTO transactionaCoreResponse = new ResponseDTO();
            BusinessActivityService _service = new BusinessActivityService(Convert.ToString(terminalID), ref transactionaCoreResponse);
            Console.WriteLine("\n");
            if (!_service.Available())
            {
                Console.WriteLine("Servicio no disponible.");
                Console.WriteLine("-> Verificar la configuración en EmuladorCajero.exe.Config");
                Console.ReadLine();
                return;
            }
            while (!salida.Equals("exit"))
            {
                int dni;
                double saldoInicial = -1;
                double saldoPostDeb = -1;
                double saldoPostRev = -1;
                Console.Write("-> Ingrese DNI: ");
                while (!int.TryParse(Console.ReadLine(), out dni))
                {
                    Console.Write("Ingrese un DNI válido: ");
                }

                Console.Write("-> Ingrese Token: ");
                int token;
                while (!int.TryParse(Console.ReadLine(), out token))
                {
                    Console.Write("Ingrese un token numérico: ");
                }


                ResponseDTO response = _service.GetToken(dni, token);
                if (!response.status)
                {
                    Console.WriteLine(string.Format("{0} [{1}]", response.description, response.message));
                    Console.WriteLine("-> Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                RespuestaTokenDTO respuestaToken = (RespuestaTokenDTO)response.responseData;
                Console.WriteLine("");
                Console.WriteLine("Usuario: " + respuestaToken.clienteDTO.alias);
                foreach (CuentaDTO cuenta in respuestaToken.cuentas)
                {
                    Console.WriteLine("Cuenta: " + cuenta.numero);
                    string tipoDeCuenta = string.Empty;
                    switch (cuenta.tipoCuentaId)
                    {
                        case 19:
                            tipoDeCuenta = "Pesos";
                            break;
                        case 20:
                            tipoDeCuenta = "Temporal";
                            break;
                        default:
                            tipoDeCuenta = cuenta.tipoCuentaId.ToString();
                            break;
                    }

                    Console.WriteLine("Tipo de cuenta: " + tipoDeCuenta);
                    Console.WriteLine("Saldo total: " + cuenta.saldo);
                    saldoInicial = (double)cuenta.saldo;
                }
                Console.WriteLine("Saldo del token: " + respuestaToken.tokenCheckDTO.saldoToken);
                Console.WriteLine("Estado del token: " + respuestaToken.tokenCheckDTO.respuesta);
                Console.WriteLine("\n");
                if (respuestaToken.tokenCheckDTO.estadoToken.Equals("USADO"))
                {
                    Console.WriteLine("-> Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                Console.Write("-> Ingrese el importe que desea extraer: ");
                int importe;
                while (!int.TryParse(Console.ReadLine(), out importe))
                {
                    Console.Write("Sólo números: ");
                }
                TransactionDTO debitDTO = new TransactionDTO()
                {
                    dniOrigen = Convert.ToString(dni),
                    dniDestino = Convert.ToString(dni),
                    terminal = Convert.ToString(terminalID),
                    importe = importe,
                    idUsuario = respuestaToken.tokenCheckDTO.idUsuario,
                    tokenId = respuestaToken.tokenCheckDTO.tokenId
                };
                ResponseDTO responseDebito = _service.Debit(debitDTO);
                if (responseDebito.status)
                {
                    ResponseTransactionDTO res = (ResponseTransactionDTO)responseDebito.responseData;
                    ResponseDTO responseMovimiento = _service.GetMovimiento(res.id);
                    MovimientoDTO mov = (MovimientoDTO)responseMovimiento.responseData;
                    Console.WriteLine("\n");
                    Console.WriteLine(responseDebito.description);

                    DateTime fecha = Convert.ToDateTime(res.fecha);
                    Console.WriteLine("\nRedATM");
                    Console.WriteLine("Fecha: " + fecha.Date.ToShortDateString());
                    Console.WriteLine("Hora: " + fecha.TimeOfDay);
                    Console.WriteLine("Terminal: " + res.terminal);
                    //Console.WriteLine("Número de cuenta: " + );
                    Console.WriteLine("Número de transacción: " + res.codigo);
                    Console.WriteLine("Transacción: " + res.detalle);
                    Console.WriteLine("Importe: $" + (mov.importeOrigen - mov.transaccion.costoOperacion).ToString()); ;
                    Console.WriteLine("Costo de la transacción: $" + mov.transaccion.costoOperacion.ToString());
                    Console.WriteLine("Su saldo es (S.E.U.O): $" + mov.saldo);
                    saldoPostDeb = mov.saldo;
                    Console.WriteLine("\n");
                    Console.WriteLine("¿Desea simular una reversa?");
                    Console.Write("-> Tipee \"yes\" para confirmar, o cualquier otro caracter para continuar: ");
                    if (Console.ReadLine().Equals("yes"))
                    {
                        TransactionDTO creditDTO = new TransactionDTO()
                        {
                            dniOrigen = Convert.ToString(dni),
                            dniDestino = Convert.ToString(dni),
                            terminal = Convert.ToString(terminalID),
                            importe = (decimal)mov.importeOrigen,
                            idUsuario = respuestaToken.tokenCheckDTO.idUsuario,
                            tokenId = respuestaToken.tokenCheckDTO.tokenId
                        };
                        ResponseDTO responseCredito = _service.Debit(creditDTO, true);
                        if (responseCredito.status)
                        {
                            ResponseTransactionDTO resReversa = (ResponseTransactionDTO)responseCredito.responseData;
                            ResponseDTO responseMovimientoReversa = _service.GetMovimiento(resReversa.id);
                            MovimientoDTO movReversa = (MovimientoDTO)responseMovimientoReversa.responseData;
                            Console.WriteLine("\n");
                            Console.WriteLine("Reversa generada correctamente");
                            Console.WriteLine(responseCredito.description);
                            DateTime fechaReversa = Convert.ToDateTime(resReversa.fecha);
                            Console.WriteLine("\nRedATM REVERSA");
                            Console.WriteLine("Fecha: " + fechaReversa.Date.ToShortDateString());
                            Console.WriteLine("Hora: " + fechaReversa.TimeOfDay);
                            Console.WriteLine("Terminal: " + resReversa.terminal);
                            //Console.WriteLine("Número de cuenta: " + );
                            Console.WriteLine("Número de transacción: " + resReversa.codigo);
                            Console.WriteLine("Transacción: " + resReversa.detalle);
                            Console.WriteLine("Importe: $" + (movReversa.importeOrigen).ToString()); ;
                            Console.WriteLine("Costo de la transacción: $" + movReversa.transaccion.costoOperacion.ToString());
                            Console.WriteLine("Su saldo es (S.E.U.O): $" + movReversa.saldo);
                            Console.WriteLine("\n");
                            saldoPostRev = movReversa.saldo;
                        }
                        else
                        {
                            Console.WriteLine(string.Format("{0} [{1}]\n", responseCredito.description, responseCredito.message));
                        }
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("{0} [{1}]", responseDebito.description, responseDebito.message));
                    Console.WriteLine("-> Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                if (saldoPostDeb >= 0)
                {
                    Console.WriteLine("\nSaldo inicial:      " + saldoInicial.ToString());
                    Console.WriteLine("Saldo post-debito:  " + saldoPostDeb.ToString());
                    if (saldoPostRev >= 0)
                    {
                        Console.WriteLine("Saldo post-reversa: " + saldoPostRev.ToString() + "\n");
                    }
                }
                Console.WriteLine("-> Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                salida = Console.ReadLine();
                if (!salida.Equals("exit"))
                {
                    Console.WriteLine("--------------------------------------------------------------------------\n");
                }
            }
        }

    }
}
