﻿namespace Automation.Plugins.Shared
{
    /*
    public class TaskEndpoint
    {
        public Type Type { get; set; }
        public ITask Parent { get; set; }
        public object? Data { get; set; }

        public TaskEndpoint(Type type, ITask parent)
        {
            Type = type;
            Parent = parent;
        }

        public bool IsValid()
        {
            if (Nullable.GetUnderlyingType(Type) == null && Data == null)
                return false;
            return Data?.GetType().IsInstanceOfType(Type) == true;
        }
    }

    public class TaskBase : ITask
    {
        public string Name { get; set; }
        public dynamic? Context { get; set; }

        public Dictionary<string, TaskEndpoint> Inputs { get; set; } = [];

        public Dictionary<string, TaskEndpoint> Outputs { get; set; } = [];


        public virtual Task<bool> Start()
        {
            if (!ValidateInputs(Inputs))
                return Task.FromResult(false);
            return Task.FromResult(true);
        }

        protected virtual bool ValidateInputs(Dictionary<string, TaskEndpoint> inputs)
        {
            foreach (var input in inputs)
            {
                if (!input.Value.IsValid())
                    return false;
            }
            return true;
        }
    }
    */
}
