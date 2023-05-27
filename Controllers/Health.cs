using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;
using log4net.Repository;
using System.Reflection;
using log4net.Core;
using System.Diagnostics;


namespace ThirdAssignment_Server.Controllers
{
    public class Health : Controller
    {
        private ILog _requestsLogger;
        private ILog _toDoLogger;

        public Health()
        {
            _requestsLogger = LogManager.GetLogger("request-logger");
            _toDoLogger = LogManager.GetLogger("todo-logger");
        }

        // Get: Health
        [HttpGet]
        [Route("/todo/health")]
        public ActionResult<string> CheckHealth()
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");          
            
            //FUNCTION
            string message = "OK";
            
            //other logging 
            stopwatch.Stop();
            long elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            _requestsLogger.Debug($"request #{requestNumber} duration: {elapsedTimeMs}ms");
            return Ok(message);
        }
    }
}
