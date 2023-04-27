using Microsoft.AspNetCore.Http;
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
        [Route("Health")]
        public IActionResult CheckHealth(IFormCollection collection)
        {
            return Ok();
        }
    }
}
