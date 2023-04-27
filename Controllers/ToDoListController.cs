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
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", $"Error: TODO with the title[{jsonData.title}] already exists in the system")));
            }
            if (TimeStampIsNotGood(jsonData.dueDate))
            {
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", $"Error: Can’t create new TODO that its due date is in the past")));
            }
            ToDoTask newTask = new ToDoTask(jsonData.title, jsonData.content, jsonData.dueDate);
            toDoList.tasksList.Add(newTask);
            return StatusCode(200, JsonConvert.SerializeObject(new Response($"{newTask.uniqueId}", "")));
        }

        [HttpGet]
        [Route("/todo/size")]
        public ActionResult<string> GetToDoCount([FromQuery] string status)
        {
            int numOfToDo = countToDoByFilter(status);
            if (numOfToDo < 0)
            {
                //if error return error
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));

            }
            return StatusCode(200, JsonConvert.SerializeObject(new Response($"{numOfToDo}", "")));
        }





        public class AssignmentData
        {
            public string title { get; set; }
            public string content { get; set; }
            public long dueDate { get; set; }
            public AssignmentData(string _title, string _content, long _dueDate)
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
                if (currTitle == item.title)
                {
                    return true;
                }
            }
            return false;
        }
        public bool TimeStampIsNotGood(long dueDate)
        {
            return (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < dueDate);
        }
        public int countToDoByFilter(string status)
        {
            int count = 0;
            if (status == "ALL" || status == "LATE" || status == "PENDING" || status == "DONE")
            {
                foreach (var item in toDoList.tasksList)
                {
                    if (status == "ALL")
                    {
                        count++;
                    }
                    else
                    {
                        if (status == ToDoTask.statusStr[((int)item.status)])
                        {
                            count++;
                        }
                    }
                }
            }
            else
            {
                count = -1;
            }
            return count;
        }
    }
}
