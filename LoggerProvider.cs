using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{
    public class LoggerProvider : ILoggerProvider
    {
        private readonly string _loggerName;

        public LoggerProvider(string loggerName)
        {
            _loggerName = loggerName;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (categoryName == _loggerName)
            {
                // Create and return an instance of your custom logger
                return new MyLogger();
            }

            // Return null if this provider does not support the requested logger
            return null;
        }

        public void Dispose()
        {
            // Cleanup resources, if any
        }
    }
}
