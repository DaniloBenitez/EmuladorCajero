using EmuladorCajero.DTO;
using System;

namespace EmuladorCajero
{
    class Program
    {
        static void Main(string[] args)
        {
            string salida = string.Empty;
            while (!salida.Equals("exit"))
            {
                long terminalID = 72172129;
                ResponseDTO transactionaCoreResponse = new ResponseDTO();
                BusinessActivityService _service = new BusinessActivityService(Convert.ToString(terminalID), ref transactionaCoreResponse);
                Console.WriteLine("\n");

                int dni;
                Console.Write("Ingrese DNI: ");
                while (!int.TryParse(Console.ReadLine(), out dni))
                {
                    Console.Write("Ingrese un DNI válido: ");
                }

                Console.Write("Ingrese Token: ");
                int token;
                while (!int.TryParse(Console.ReadLine(), out token))
                {
                    Console.Write("Ingrese un token numérico: ");
                }

                if (token <= 0)
                {
                    token = 800791;
                }

                ResponseDTO response = _service.GetToken(dni, token);
                if (!response.status)
                {
                    Console.WriteLine(string.Format("{0} [{1}]", response.description, response.message));
                    Console.WriteLine("Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                RespuestaTokenDTO respuestaToken = (RespuestaTokenDTO)response.responseData;
                Console.WriteLine("Usuario: " + respuestaToken.clienteDTO.alias);
                Console.WriteLine("Saldo del token: " + respuestaToken.tokenCheckDTO.saldoToken);
                Console.WriteLine("Estado del token: " + respuestaToken.tokenCheckDTO.respuesta);
                Console.WriteLine("\n");
                if (respuestaToken.tokenCheckDTO.estadoToken.Equals("USADO"))
                {
                    Console.WriteLine("Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                Console.Write("Ingrese el importe que desea extraer: ");
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
                    Console.WriteLine("\n");
                    Console.WriteLine(responseDebito.description);
                    Console.WriteLine("");
                    Console.WriteLine("       Código de la operación: " + res.codigo);
                    Console.WriteLine("       Tipo de operación: " + res.detalle);
                    Console.WriteLine("       Fecha de la operación: " + res.fecha);
                    Console.WriteLine("       Token remanente: " + res.numeroToken);
                    Console.WriteLine("\n");
                }
                else
                {
                    Console.WriteLine(string.Format("{0} [{1}]", responseDebito.description, responseDebito.message));
                    Console.WriteLine("Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                    salida = Console.ReadLine();
                    continue;
                }
                Console.WriteLine("Tipee \"exit\" para salir, o cualquier otro caracter para continuar...");
                salida = Console.ReadLine();
            }
        }

    }
}
