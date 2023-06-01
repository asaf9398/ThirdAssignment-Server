using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using log4net;
using System.Diagnostics;

namespace ThirdAssignment_Server.Controllers
{
    public class ToDoListController : Controller
    {
        public static ToDoList toDoList = new ToDoList();
        private ILog _requestsLogger;
        private ILog _toDoLogger;
        public ToDoListController()
        {
            _requestsLogger = LogManager.GetLogger("request-logger");
            _toDoLogger = LogManager.GetLogger("todo-logger");
        }

        [HttpPost]
        [Route("/todo")]
        public ActionResult<string> CreateNewToDo([FromBody] AssignmentData jsonData)
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            string errorMessage;

            if (jsonData == null)
            {
                StopWatchAndWriteLog(stopwatch, requestNumber);
                errorMessage = $"Bad request - unvalid json entered";
                return StatusCode(400, JsonConvert.SerializeObject(new Response("", errorMessage)));

            }
            if (IsAlreadyTaken(jsonData.title))
            {
                errorMessage = $"Error: TODO with the title[{jsonData.title}] already exists in the system";
                _toDoLogger.Error(errorMessage);
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", errorMessage)));
            }
            if (!IsTimeStampGood(jsonData.dueDate))
            {
                errorMessage = $"Error: Can’t create new TODO that its due date is in the past";
                _toDoLogger.Error(errorMessage);
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(409, JsonConvert.SerializeObject(new Response("", errorMessage)));
            }
            int lastCountBeforeAdding = toDoList.tasksList.Count;
            ToDoTask newTask = new ToDoTask(jsonData.title, jsonData.content, jsonData.dueDate);
            toDoList.tasksList.Add(newTask);

            _toDoLogger.Info($"Creating new TODO with Title [{jsonData.title}]");
            _toDoLogger.Debug($"Currently there are {lastCountBeforeAdding} TODOs in the system. New TODO will be assigned with id {newTask.id}");

            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(newTask.id, "")));
        }

        [HttpGet]
        [Route("/todo/size")]
        public ActionResult<string> GetToDoCount([FromQuery] string status)
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");

            int numOfToDo = countToDoByFilter(status);
            if (numOfToDo < 0)
            {
                //if error return error
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));
            }
            _toDoLogger.Info($"Total TODOs count for state {status} is {numOfToDo}");
            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(numOfToDo, "")));
        }


        [HttpGet]
        [Route("/todo/content")]
        public ActionResult<string> GetToDoContent([FromQuery] string status, [FromQuery] string sortBy = "ID")
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            string errorMessage = "";

            if (!IsLegalStatus(status) || !IsLegalSortBy(sortBy))
            {
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, JsonConvert.SerializeObject(new Response(Array.Empty<ToDoContent>(), errorMessage)));
            }

            if (ToDoTask.nextId == 1)
            {
                //if there are no elements yet
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(200, JsonConvert.SerializeObject(new Response(Array.Empty<ToDoContent>(), "")));
            }
            _toDoLogger.Info($"Extracting todos content. Filter: {status} | Sorting by: {sortBy}");
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
            _toDoLogger.Debug($"There are a total of {toDoList.tasksList.Count} todos in the system. The result holds {toDoContentArray.Length} todos");
            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(toDoContentArray, "")));
        }

        [HttpPut]
        [Route("/todo")]
        public ActionResult<string> UpdateToDo([FromQuery] int id, [FromQuery] string status)
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            _toDoLogger.Info($"Update TODO id [{id}] state to {status}");

            if (!IsLegalStatus(status) || status == "ALL")
            {
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));
            }
            if (id == 0)
            {
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(400, JsonConvert.SerializeObject(new Response()));
            }

            if (!ThereIsToDoWithID(id))
            {
                _toDoLogger.Error($"Error: no such TODO with id {id}");
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(404, JsonConvert.SerializeObject(new Response("", $"Error: no such TODO with id {id}")));
            }

            _toDoLogger.Debug($"Todo id [{id}] state change: {GetOldStatus(id)} --> {status}");

            string oldStatus = UpdateStatus(id, status);
            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(oldStatus, "")));
        }

        [HttpDelete]
        [Route("/todo")]
        public ActionResult<string> DeleteToDo([FromQuery] int id)
        {
            //logging
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            int requestNumber = RequestsCounter.GetCounter();
            log4net.GlobalContext.Properties["request-number"] = requestNumber;
            _requestsLogger.Info($"Incoming request | #{requestNumber} | resource: {HttpContext.Request.Path} | HTTP Verb {HttpContext.Request.Method}");
            string errorMessage;

            if (!ThereIsToDoWithID(id))
            {
                errorMessage = $"Error: no such TODO with id {id}";
                _toDoLogger.Error(errorMessage);
                StopWatchAndWriteLog(stopwatch, requestNumber);
                return StatusCode(404, JsonConvert.SerializeObject(new Response("", errorMessage)));
            }
            _toDoLogger.Info($"Removing todo id {id}");

            DeleteToDoFromList(id);
            int leftToDoInList = toDoList.tasksList.Count;

            _toDoLogger.Debug($"After removing todo id [{id}] there are {leftToDoInList} TODOs in the system");

            StopWatchAndWriteLog(stopwatch, requestNumber);
            return StatusCode(200, JsonConvert.SerializeObject(new Response(leftToDoInList, "")));
        }



        void StopWatchAndWriteLog(Stopwatch stopwatch, int requestNumber)
        {
            stopwatch.Stop();
            long elapsedTimeMs = stopwatch.ElapsedMilliseconds;
            _requestsLogger.Debug($"request #{requestNumber} duration: {elapsedTimeMs}ms");
        }
        string GetOldStatus(int id)
        {
            string oldStatus = "";
            foreach (var item in toDoList.tasksList)
            {
                if (item.id == id)
                {
                    oldStatus = item.status;
                    break;
                }
            }
            return oldStatus;
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
