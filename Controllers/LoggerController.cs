using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using System.Reflection;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using System.Diagnostics;

namespace ThirdAssignment_Server.Controllers
{
    public class LoggerController : Controller
    {
        private ILog _requestsLogger;
        public LoggerController()
        {
            _requestsLogger = LogManager.GetLogger("request-logger");         
        }

        [HttpGet]
        [Route("/logs/level")]
        public ActionResult<string> GetLevel([FromQuery(Name = "logger-name")] string loggerName)
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            string errorMessage;

            if (loggerName != "request-logger" && loggerName != "todo-logger")
            {
                errorMessage = $"No such logger";
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, errorMessage);
            }
            ILog logger = LogManager.GetLogger(loggerName);
            ILoggerRepository repository = LogManager.GetRepository();
            Logger loggerObject = (Logger)repository.GetLogger(logger.Logger.Name);
            string levelName="";
            int levelValue;
            var loggerLevel = loggerObject.Level;

            if (loggerLevel != null)
            {
                // Access the logger level properties
                levelName = loggerLevel.Name; // e.g., "INFO", "DEBUG", etc.
                levelValue = loggerLevel.Value; // e.g., 20000, 10000, etc.
                                                // ...
            }
            else
            {
                errorMessage = $"Error while getting level from {loggerName}";
                return StatusCode(409, errorMessage);
            }

            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, $"{levelName}");
        }


        [HttpPut]
        [Route("/logs/level")]
        public ActionResult<string> SetLevel([FromQuery(Name = "logger-name")] string loggerName, [FromQuery(Name = "logger-level")] string newLoggerLevel)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            string errorMessage;

            if (loggerName != "request-logger" && loggerName != "todo-logger")
            {
                errorMessage = $"No such logger named {loggerName}";               
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, errorMessage);
            }

            if (newLoggerLevel != "ERROR" && newLoggerLevel != "DEBUG" && newLoggerLevel != "INFO")
            {
                errorMessage = $"No such level \"{newLoggerLevel}\"";              
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, errorMessage);
            }

            ILog logger = LogManager.GetLogger(loggerName);
            ILoggerRepository repository = LogManager.GetRepository();
            Logger loggerObject = (Logger)repository.GetLogger(logger.Logger.Name);
            if (loggerObject == null)
            {
                errorMessage = $"Error while getting {loggerName} logger object";                
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(409, errorMessage);
            }

            // Get the log level from the LevelMap
            Level level = loggerObject.Hierarchy.LevelMap[newLoggerLevel];
           
            if (level==null)
            {
                errorMessage = $"Error while getting {loggerName} level property";               
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(409, errorMessage);
            }
            // Set the log level for the logger
            loggerObject.Level = level;                    
            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, $"{newLoggerLevel}");
        }
        void StopWatchAndWriteLog(Stopwatch stopwatch, int requestNumber)
        {
            stopwatch.Stop();
            long elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            _requestsLogger.Debug($"request #{requestNumber} duration: {elapsedTimeMs}ms");
        }
    }
}
