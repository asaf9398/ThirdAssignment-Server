﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server.Controllers
{
    public class Health : Controller
    { 
        // Get: Health
        [HttpGet]
        [Route("/todo/health")]
        public ActionResult<string> CheckHealth()
        {
            string message = "OK";
            return Ok(message);
        }
    }
}
