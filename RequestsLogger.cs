using log4net;
using log4net.Repository;



namespace ThirdAssignment_Server
{
    public static class RequestsLogger
    {
        private static ILog _requestsLogger= LogManager.GetLogger("request-logger");
        private static string _level = "INFO";
        private static ILoggerRepository level;

        public static ILog Logger()
        {
            return _requestsLogger;          
        }

        public static void SetLevel(string level)
        {


        }
    }
}
