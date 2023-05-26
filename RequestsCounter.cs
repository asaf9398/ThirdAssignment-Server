using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{
    public static class RequestsCounter
    {
        private static int requestsCounter = 1;
        public static int GetRequsetsCounter()
        {
            int oldCounter = requestsCounter;
            requestsCounter++;
            return oldCounter;
        }
    }
}
