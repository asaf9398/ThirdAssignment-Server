using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{
    public static class RequestsCounter
    {
        private static int counter = 1;
        public static int GetCounter()
        {
            int oldCounter = counter;
            counter++;
            return oldCounter;
        }
    }
}
