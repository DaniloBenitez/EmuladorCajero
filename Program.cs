using EmuladorCajero.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmuladorCajero
{
    class Program
    {
        static void Main(string[] args)
        {
            ResponseDTO transactionaCoreResponse = new ResponseDTO();
            BusinessActivityService _service = new BusinessActivityService(Convert.ToString(72172128), ref transactionaCoreResponse);
        }
    }
}
