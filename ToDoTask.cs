using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThirdAssignment_Server
{

    public enum Status { PENDING, LATE, DONE }
    public class ToDoTask
    {
        static string[] statusStr = { "PENDING", "LATE", "DONE" };
        public int uniqueId { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public DateTime dueDate { get; set; }
        public Status status { get; set; }
        public ToDoTask(int _uniqueId,string _title,string _content,DateTime _dueDate)
        {
            uniqueId = _uniqueId;
            title = _title;
            content = _content;
            dueDate = _dueDate;
            status = Status.PENDING;
        }

    }
}
