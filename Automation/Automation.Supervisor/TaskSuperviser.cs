﻿using Automation.Plugins.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation.Supervisor
{
    public class  TaskSupervisorParams
    {
        public List<string> DllsFolderPaths { get; set; }
    }

    /// <summary>
    /// Handle DLL resolution
    /// Handle load balancing
    /// Execute tasks with a TaskWorker
    /// </summary>
    public class TaskSuperviser
    {
        private readonly TaskSupervisorParams Params;
        public List<Type> AvailableTasks { get; private set; }

        public TaskSuperviser(TaskSupervisorParams taskSupervisorParams)
        {
            Params = taskSupervisorParams;
            AvailableTasks = LoadDllsClasses(Params.DllsFolderPaths);
        }

        private List<Type> LoadDllsClasses(List<string> dllsFolderPaths)
        {
            // Load all DLLs from the specified folder and get all classes that implement ITask
            List<Type> availableTasks = new List<Type>();
            foreach (var dllPath in dllsFolderPaths)
            {
                var dll = System.Reflection.Assembly.LoadFile(dllPath);
                
                foreach (var type in dll.GetTypes())
                {
                    if (type.IsAssignableFrom(typeof(ITask)))
                        availableTasks.Add(type);
                }
            }
            return availableTasks;
        }
    }
}