

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
