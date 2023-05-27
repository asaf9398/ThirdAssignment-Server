using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using log4net;

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
            _requestsLogger.Info("testing requests logger");
            _toDoLogger.Info("testing todo logger");
            string message = "OK";
            return Ok(message);
        }
    }
}
