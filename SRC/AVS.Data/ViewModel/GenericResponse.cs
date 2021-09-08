using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVS.Data.ViewModel
{
    public class GenericResponse
    {
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public AVSDetails Data { get; set; }
    }

    public class AVSDetails
    {
        public string AccountNumber { get; set; }
        public string Accountname { get; set; }
    }
}
