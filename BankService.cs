using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using EmuladorCajero.DTO;
using EmuladorCajero;

namespace BankInterface
{
    internal class BankService : IDisposable
    {
        private string _host;
        private string _token;
        private int _tries;

        public int Tries
        {
            get { return _tries; }
            set { _tries = value; }
        }
        private static ManualResetEvent allDone = new ManualResetEvent(false);
        const int BUFFER_SIZE = 1024;
        const int MAX_TRIES = 3;

        public BankService(string host, string user, string pass, bool rememberMe, ref ResponseDTO r)
        {
           // test(new Uri(host), user, pass);
            _host = host;
            Hashtable parametros = new Hashtable();
            Console.WriteLine("Autenticando usuario: " + user);
            parametros.Add("username", user);
            parametros.Add("password", pass);
            parametros.Add("rememberMe", rememberMe);
            Hashtable respuesta = Post("api/authenticate", parametros);
             r = new ResponseDTO();
            Mapper.MapObject(respuesta,r);
            _token = Convert.ToString(respuesta["id_token"]);
            r.status = !string.IsNullOrWhiteSpace(_token);
            if (!r.status && string.IsNullOrWhiteSpace(r.message))
            {
                r.message = "Se produjo un error al obtener el token";
            }
            if (!r.status)
            {
                Console.WriteLine(r.message);
            } else
            {
                Console.WriteLine(user + " autenticado correctamente");
            }

        }
        public Hashtable Get(string pRequestName, Dictionary<string, string> pParams = null)
        {
            _tries = 0;
            return Call(pRequestName, "GET", pParams, null);
        }
        public Hashtable Post(string pRequestName, Hashtable pData)
        {
            _tries = 0;
            return Call(pRequestName, "POST", null, pData);
        }
        public Hashtable Put(string pRequestName, Hashtable pData = null)
        {
            _tries = 0;
            return Call(pRequestName, "PUT", null, pData);
        }

        private Hashtable Call(string pRequestName, string pMethod, Dictionary<string, string> pParams, Hashtable pData)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateDefault(GetUri(pParams, pRequestName)) as HttpWebRequest;
                request.Method = pMethod;
                request.ContentType = "application/json";
                request.ProtocolVersion = HttpVersion.Version10;
                request.KeepAlive = false;
                System.Net.ServicePointManager.DefaultConnectionLimit = 200;

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls;
                if (_token != null)
                    request.Headers.Set(HttpRequestHeader.Authorization, "Bearer " + _token);
                
                ASCIIEncoding encoder = new ASCIIEncoding();
                byte[] data = null;
                if (pData != null){
                    data = encoder.GetBytes(JSON.JsonEncode(pData));
                    request.ContentLength = data.Length;
                }
                else
                    request.ContentLength = 0;
                
                //request.Timeout = 10000;
                if (pData != null){
                    request.AllowWriteStreamBuffering = false;
                    using (var stream = request.GetRequestStream()){
                        stream.Write(data, 0, data.Length);
                    }
                }
                return GetResponse(request);
            }
            catch (Exception ex)
            {
                _tries++;

                if (_tries <= 3)
                {
                    return Call(pRequestName, pMethod, pParams, pData);
                }
                else
                {
                    Hashtable error = new Hashtable();
                    error.Add("status", false);
                    error.Add("code", 400);
                    error.Add("message", ex.Message);

                    return error;
                }
            }
        }

        private Uri GetUri(Dictionary<string, string> pParams, string pRequestName)
        {
            string uri = string.Format("{0}{1}?", _host, pRequestName);
            if (pParams != null && pParams.Count > 0)
            {
                foreach (var item in pParams)
                {
                    uri += string.Format("{0}={1}&", item.Key, item.Value != null ? System.Uri.EscapeDataString(item.Value) : item.Value);
                }
                uri = uri.Substring(0, uri.Length - 1);
            }
            return new Uri(uri);
        }
        private Hashtable GetResponse(HttpWebRequest request)
        {
            string outputDataStr = null;
            Hashtable data = new Hashtable();
            using (HttpWebResponse myWebResponse = request.GetResponse() as HttpWebResponse)
            {
                request.ServicePoint.MaxIdleTime = 0;
               
                using (Stream streamResponse = myWebResponse.GetResponseStream())
                {
                    using (StreamReader streamRead = new StreamReader(streamResponse))
                    {
                        Char[] readBuff = new Char[BUFFER_SIZE];
                        int count = streamRead.Read(readBuff, 0, BUFFER_SIZE);

                        while (count > 0)
                        {
                            outputDataStr += new String(readBuff, 0, count);
                            count = streamRead.Read(readBuff, 0, BUFFER_SIZE);
                        }
                        if (!string.IsNullOrWhiteSpace(outputDataStr))
                        {
                            data = (Hashtable)JSON.JsonDecode(outputDataStr);
                        }
                        streamRead.Close();
                        streamResponse.Close();
                        myWebResponse.Close();
                    }
                }
            }
            return data;
        }
        
        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}
