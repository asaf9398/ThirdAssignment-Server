﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
