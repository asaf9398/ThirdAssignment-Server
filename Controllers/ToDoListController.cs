using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server.Controllers
{
    public class ToDoListController : Controller
    {
        [HttpPost]
        [Route("/todo")]
        public ActionResult<string> CreateNewToDo()
        {
            string message = "OK";
            return Ok(message);
        }
    }
}
