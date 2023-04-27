using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{

    public enum Status { PENDING, LATE, DONE }
    public class ToDoTask
    {
        static int nextId = 1;
        public static string[] statusStr = { "PENDING", "LATE", "DONE" };
        public int uniqueId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public long dueDate { get; set; }
        public Status status { get; set; }
        public ToDoTask(string _title,string _content,long _dueDate)
        {
            uniqueId = nextId;
            title = _title;
            content = _content;
            dueDate = _dueDate;
            status = Status.PENDING;
            nextId++;
        }

    }
}
