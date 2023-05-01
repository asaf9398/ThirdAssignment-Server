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
        public static ToDoList toDoList = new ToDoList();

        [HttpPost]
        [Route("/todo")]
        public ActionResult<string> CreateNewToDo([FromBody] AssignmentData jsonData)
        {
            if (jsonData==null)
            {
                return StatusCode(400, JsonConvert.SerializeObject(new Response("", $"Bad request - unvalid json entered")));

            }
            if (IsAlreadyTaken(jsonData.title))
            {
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", $"Error: TODO with the title[{jsonData.title}] already exists in the system")));
            }
            if (!IsTimeStampGood(jsonData.dueDate))
            {
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", $"Error: Can’t create new TODO that its due date is in the past")));
            }
            ToDoTask newTask = new ToDoTask(jsonData.title, jsonData.content, jsonData.dueDate);
            toDoList.tasksList.Add(newTask);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(newTask.id, "")));
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
            return StatusCode(200, JsonConvert.SerializeObject(new Response(numOfToDo, "")));
        }


        [HttpGet]
        [Route("/todo/content")]
        public ActionResult<string> GetToDoContent([FromQuery] string status, [FromQuery] string sortBy = "ID")
        {
            if (!IsLegalStatus(status) || !IsLegalSortBy(sortBy))
            {
                return StatusCode(400, JsonConvert.SerializeObject(new Response(Array.Empty<ToDoContent>(), "")));
            }

            if (ToDoTask.nextId == 1)
            {
                //if there are no elements yet
                return StatusCode(200, JsonConvert.SerializeObject(new Response(Array.Empty<ToDoContent>(), "")));
            }
            ToDoContent[] toDoContentArray = GetToDoContentArray(toDoList.tasksList, status);
            switch (sortBy)
            {
                case "ID":
                    Array.Sort(toDoContentArray, CompareByID);
                    break;

                case "DUE_DATE":
                    Array.Sort(toDoContentArray, CompareByDueDate);
                    break;

                case "TITLE":
                    Array.Sort(toDoContentArray, CompareByTitle);
                    break;

                default:
                    break;
            }

            return StatusCode(200, JsonConvert.SerializeObject(new Response(toDoContentArray, "")));
        }

        [HttpPut]
        [Route("/todo")]
        public ActionResult<string> UpdateToDo([FromQuery] int id, [FromQuery] string status)
        {
            if (!IsLegalStatus(status) || status == "ALL")
            {
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));
            }
            if (id==0)
            {
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));
            }

            if (!ThereIsToDoWithID(id))
            {
                return StatusCode(404, JsonConvert.SerializeObject(new Response("", $"Error: no such TODO with id {id}")));
            }
            string oldStatus = UpdateStatus(id, status);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(oldStatus, "")));
        }

        [HttpDelete]
        [Route("/todo")]
        public ActionResult<string> DeleteToDo([FromQuery] int id)
        {
            if (!ThereIsToDoWithID(id))
            {
                return StatusCode(404, JsonConvert.SerializeObject(new Response("", $"Error: no such TODO with id {id}")));
            }
            DeleteToDoFromList(id);
            int leftToDoInList = toDoList.tasksList.Count;
            return StatusCode(200, JsonConvert.SerializeObject(new Response(leftToDoInList, "")));
        }
        void DeleteToDoFromList(int id)
        {
            foreach (var item in toDoList.tasksList)
            {
                if (item.id == id)
                {
                    toDoList.tasksList.Remove(item);
                    break;
                }
            }
        }
        string UpdateStatus(int id, string newStatus)
        {
            string oldStatus = "";
            foreach (var item in toDoList.tasksList)
            {
                if (item.id == id)
                {
                    oldStatus = item.status;
                    switch (newStatus)
                    {
                        case "PENDING":
                            item.statusIndex = Status.PENDING;
                            break;
                        case "LATE":
                            item.statusIndex = Status.LATE;
                            break;
                        case "DONE":
                            item.statusIndex = Status.DONE;
                            break;
                        default:
                            break;
                    }
                    item.status = ToDoTask.statusStr[(int)item.statusIndex];
                    break;
                }
            }
            return oldStatus;
        }
        bool ThereIsToDoWithID(int id)
        {
            foreach (var item in toDoList.tasksList)
            {
                if (item.id == id)
                {
                    return true;
                }
            }
            return false;
        }
        bool IsLegalSortBy(string sortBy)
        {
            return (sortBy == "ID" || sortBy == "DUE_DATE" || sortBy == "TITLE");
        }
        bool IsLegalStatus(string status)
        {
            return (status == "ALL" || status == "PENDING" || status == "LATE" || status == "DONE");
        }
        int CompareByID(ToDoContent a, ToDoContent b)
        {
            return (a.id - b.id);
        }
        int CompareByDueDate(ToDoContent a, ToDoContent b)
        {
            return (int)(a.dueDate - b.dueDate);
        }
        int CompareByTitle(ToDoContent a, ToDoContent b)
        {
            return (a.title.CompareTo(b.title));
        }

        public ToDoContent[] GetToDoContentArray(List<ToDoTask> toDoList, string status)
        {
            int counter = 0;
            foreach (var item in toDoList)
            {
                if (status == "ALL")
                {
                    counter++;
                }
                else if (item.status == status)
                {
                    counter++;
                }
            }
            if (counter == 0)
            {
                return Array.Empty<ToDoContent>();
            }
            ToDoContent[] array = new ToDoContent[counter];
            int index = 0;
            foreach (var item in toDoList)
            {
                if (status == "ALL")
                {
                    array[index] = new ToDoContent(item);
                    index++;
                }
                else if (item.status == status)
                {
                    array[index] = new ToDoContent(item);
                    index++;
                }
            }
            return array;
        }
        public class ToDoContent
        {
            public int id { get; set; }
            public string title { get; set; }
            public string content { get; set; }
            public string status { get; set; }
            public long dueDate { get; set; }
            public ToDoContent(ToDoTask task)
            {
                id = task.id;
                title = task.title;
                content = task.content;
                status = task.status;
                dueDate = task.dueDate;
            }
        }
        public class AssignmentData
        {
            public string title { get; set; }
            public string content { get; set; }
            public long dueDate { get; set; }
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
        public bool IsTimeStampGood(long dueDate)
        {
            return (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() < dueDate);
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
                        if (status == ToDoTask.statusStr[((int)item.statusIndex)])
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
