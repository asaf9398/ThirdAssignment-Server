using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ThirdAssignment_Server.Controllers
{
    public class Health : Controller
    {
        private readonly ILogger<Health> _requestsLogger;
        private readonly ILogger<Health> _toDoLogger;
        
        public Health(ILogger<Health> requestsLogger, ILogger<Health> toDoLogger)
        {
            _requestsLogger = requestsLogger;
            _toDoLogger = toDoLogger;
        }

        // Get: Health
        [HttpGet]
        [Route("/todo/health")]
        public ActionResult<string> CheckHealth()
        {
            string message = "OK";
            _requestsLogger.LogInformation($"Incoming request | #{RequestsCounter.GetRequsetsCounter()} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            //_toDoLogger.LogInformation($"Incoming request | #{RequestsCounter.GetRequsetsCounter()} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            return Ok(message);
        }
    }
}
