using System;
using System.Collections;
using System.Collections.Generic;
using EmuladorCajero.DTO;

namespace EmuladorCajero
{
    public class BusinessActivityService 
    {
        BankService _service;
        public BusinessActivityService(string terminalId, ref ResponseDTO res)
        {
            var host = eCatConfig.Host();
            var registered = eCatConfig.GetValue("Registered");

            if (registered == null || registered == "0")
                Registrar(terminalId);
            else
                Console.WriteLine("Cajero registrado");

            var user = eCatConfig.GetValue("ATMUser");
            var pass = eCatConfig.GetValue("ATMPass");
            _service = new BankService(host, user,pass, true, ref res);
            
        }   
        public ResponseDTO GetToken(long dni, long  token)
        {
            RespuestaTokenDTO res = new RespuestaTokenDTO();
            Hashtable data = _service.Get(string.Format("tokenotp/api/token-otps/{0}/dni/{1}",Convert.ToString(token).PadLeft(6,'0'),dni));
            return Mapper.MapResponse(res, data);
        }
        
        public void Registrar(string terminalId)
        {
            Console.WriteLine("Registrando ATM " + terminalId);
            var res = new ResponseDTO();
            var host = eCatConfig.Host();
            var user = eCatConfig.GetValue("AdminUser");
            var pass = eCatConfig.GetValue("AdminPass");
            _service = new BankService(host, user, pass, false, ref res);
            if (res.status)
            {
                Random rnd = new Random();
                string newPass = "";
                string newUser = "atm" + terminalId;
                for (int i = 0; i < 3; i++)
                {
                    newPass += (char)rnd.Next('a', 'z');
                    newPass += (char)rnd.Next('0', '9');
                    newPass += (char)rnd.Next('A', 'Z');
                }
                Hashtable parametros = new Hashtable();
                parametros.Add("activated", true);
                parametros.Add("login", newUser);
                parametros.Add("password", newPass);
                parametros.Add("cuitDelComercio", eCatConfig.GetValue("CuitComercio"));

                Hashtable data = _service.Post("api/users/registerATM", parametros);
                res = Mapper.MapResponse(res, data);
                if (res.status)
                {
                    eCatConfig.AddValue("ATMUser", newUser);
                    eCatConfig.AddValue("ATMPass", newPass);
                    eCatConfig.AddValue("Registered", "1");
                    //eCatConfig.ProtectConfiguration();
                }
                else
                {
                    System.Console.WriteLine(res.message);
                }
            }            
        }
       
        public ResponseDTO GetClientByDNI(long dni)
        {
            ClienteDTO res = new ClienteDTO();
            Hashtable data = _service.Get(string.Format("gestionclientes/api/cliente-bases/dni/{0}",dni));
            return Mapper.MapResponse(res, data);
        }
        public ResponseDTO GetCuentaByDNI(long dni)
        {
            CuentasClienteDTO res = new CuentasClienteDTO();
            Hashtable data = _service.Get(string.Format("mscuentatransaccion/api/cuentas/cliente-base/{0}", dni));
            return Mapper.MapResponse(res, data);
            
        }
        public ResponseDTO Debit(TransactionDTO pTransaction, bool isCredit = false)
        {
            Hashtable parameters = new Hashtable();

            parameters.Add("fecha", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ"));
            parameters.Add("origenSolicitud", "A");
            parameters.Add("origenRegistro", "atm");
            parameters.Add("terminal", pTransaction.terminal);
            parameters.Add("importe", pTransaction.importe);
            parameters.Add("tokenId", pTransaction.tokenId);
            parameters.Add("idUsuario", pTransaction.idUsuario);
            if (isCredit)
            {
                parameters.Add("dniDestino", pTransaction.dniDestino);
                parameters.Add("comision", pTransaction.comision);
                parameters.Add("idAsociadoALaTransaccion", pTransaction.idAsociadoALaTransaccion);
            }
            else
            {
                parameters.Add("dniOrigen", pTransaction.dniOrigen);
            }

            List<ResponseTransactionDTO> res = new List<ResponseTransactionDTO>();
            Hashtable data = new Hashtable();

            if (isCredit)
            {
                data = _service.Post("mscuentatransaccion/api/tansaccion/creditoReversoATestear", parameters);
            }
            else
            {
                data = _service.Post("mscuentatransaccion/api/tansaccion/retiro", parameters);
            }

            ResponseDTO respuesta = Mapper.MapResponse(res, data);

            return respuesta;
        }

        public ResponseDTO GetMovimiento(double transactionId)
        {
            MovimientoDTO res = new MovimientoDTO();
            Hashtable data = _service.Get(string.Format("mscuentatransaccion/api/movimientos/transaccion/{0}", transactionId));
            return Mapper.MapResponse(res, data);
        }

        public bool Available()
        {
            return _service.Available();
        }
    }
}
