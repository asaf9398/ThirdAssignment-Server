using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Core;
using System.Reflection;

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
            ILog logger = LogManager.GetLogger(Assembly.GetExecutingAssembly(),loggerName);
            //Level loggerLevel = ((ILogger)logger.Logger).Level;
            //Level loggerLevel=(logger.Logger).Repository.
            //log4net.Config.ConfiguratorAttribute
            return StatusCode(200, "No such logger");
        }
    }
}
