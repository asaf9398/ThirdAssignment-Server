using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{
    public class Response
    {
        public string result { get; set; }
        public string errorMessage { get; set; }
        public Response(string _result,string _errorMessage)
        {
            result = _result;
            errorMessage = _errorMessage;
        }
        public Response()
        {
            result = "";
            errorMessage = "";
        }
    }
}
