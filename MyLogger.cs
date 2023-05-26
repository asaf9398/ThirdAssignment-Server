using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ThirdAssignment_Server
{
    public class MyLogger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            // Perform logging logic here
            // You can use the logLevel, eventId, state, exception, and formatter parameters

            // Example: Logging to the console
            Console.WriteLine($"[{logLevel}] - {formatter(state, exception)}");
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            // Determine if logging is enabled for the specified log level
            // You can return true or false based on your own criteria

            // Example: Enable all log levels
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            // Begin a new logging scope, if supported
            // You can return null or a custom disposable object if needed

            return null;
        }
    }
}
