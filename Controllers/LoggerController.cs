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

namespace ThirdAssignment_Server.Controllers
{
    public class LoggerController : Controller
    {
        [HttpGet]
        [Route("/logs/level")]
        public ActionResult<string> GetLevel([FromQuery(Name = "logger-name")] string loggerName)
        {
            if (loggerName != "request-logger" && loggerName != "todo-logger")
            {
                return StatusCode(400, "No such logger");
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
            return StatusCode(200, $"{levelName}");
        }


        [HttpPut]
        [Route("/logs/level")]
        public ActionResult<string> SetLevel([FromQuery(Name = "logger-name")] string loggerName, [FromQuery(Name = "logger-level")] string newLoggerLevel)
        {
            if (loggerName != "request-logger" && loggerName != "todo-logger")
            {
                return StatusCode(400, "No such logger");
            }
            if (newLoggerLevel != "ERROR" && newLoggerLevel != "DEBUG" && newLoggerLevel != "INFO")
            {
                return StatusCode(400, "No such level");
            }
            ILog logger = LogManager.GetLogger(loggerName);
            ILoggerRepository repository = LogManager.GetRepository();
            Logger loggerObject = (Logger)repository.GetLogger(logger.Logger.Name);

            // Get the log level from the LevelMap
            Level level = loggerObject.Hierarchy.LevelMap[newLoggerLevel];

            // Set the log level for the logger
            loggerObject.Level = level;


            return StatusCode(200, $"{newLoggerLevel}");
        }
    }
}
