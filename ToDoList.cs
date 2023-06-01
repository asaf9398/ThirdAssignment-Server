using System.Collections.Generic;

namespace ThirdAssignment_Server
{
    public class ToDoList
    {
        public List<ToDoTask> tasksList { get; set; }

        public ToDoList()
        {
            tasksList = new List<ToDoTask>();
        }

    }
}
