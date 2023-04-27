using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ThirdAssignment_Server.Controllers
{
    public class ToDoListController : Controller
    {
        ToDoList toDoList = new ToDoList();

        [HttpPost]
        [Route("/todo")]
        public ActionResult<string> CreateNewToDo([FromBody] AssignmentData jsonData)
        {
            if (IsAlreadyTaken(jsonData.title))
            {
                return Ok("Test");
            }
            return Ok("Test");
        }
        public class AssignmentData
        {
            public string title { get; set; }
            public string content { get; set; }
            public long dueDate { get; set; }
            public AssignmentData(string _title,string _content,long _dueDate)
            {
                title = _title;
                content = _content;
                dueDate = _dueDate;
            }
        }
        public bool IsAlreadyTaken(string currTitle)
        {
            foreach (var item in toDoList.tasksList)
            {
                if (currTitle==item.title)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
