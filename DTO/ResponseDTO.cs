using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmuladorCajero.DTO
{
    public class ResponseDTO
    {
        public bool status {get; set;}
        public double code {get; set;}
        public string message {get; set;}
        public string description {get; set;} 
        public object responseData {get; set;} 
    }
}
      