﻿namespace Automation.Plugins.Shared
{
    public enum TaskProgressType
    {
        Info,
        Warning,
        Error,
        Sucess
    }

    public class TaskProgress
    {
        public string Message { get; set; } = string.Empty;
        public TaskProgressType Type { get; set; }
    }

    public interface IProgress
    {
        public void Progress(TaskProgress progress);
    }

    public class TaskContext
    {
        public List<Dictionary<string, object>> Scopes { get; set; } = [];
        public string? SettingsJson { get; set; }
    }

    public interface ITask
    {
        public IProgress? Progress { get; set; }
        public Task ExecuteAsync(TaskContext context);
    }

    public interface IResultsTask : ITask
    {
        public Dictionary<string, object> Results { get; }
    }
}
