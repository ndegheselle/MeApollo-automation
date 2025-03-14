﻿namespace Automation.Shared.Data
{
    public enum EnumTaskConnectorType
    {
        Data,
        Flow
    }

    public enum EnumTaskConnectorDirection
    {
        In,
        Out
    }

    public class TaskSchedule
    {
        public Guid TaskId { get; set; }
        public Schedule Schedule { get; set; }

        public TaskSchedule(Guid taskId, Schedule schedule)
        {
            TaskId = taskId;
            Schedule = schedule;
        }
    }

    public class Schedule
    {
        public string CronExpression { get; set; } = "";
        public string JsonSettings { get; set; } = "";
    }

    public interface IAutomationConnector
    {
        Guid Id { get; }
        string Name { get; set; }
        EnumTaskConnectorType Type { get; set; }
        EnumTaskConnectorDirection Direction { get; set; }
    }

    public interface IAutomationTask : IScopedElement
    {
        TargetedPackage? Package { get; set; }
        IEnumerable<IAutomationConnector> Inputs { get; }
        IEnumerable<IAutomationConnector> Outputs { get; }
        IEnumerable<Schedule> Schedules { get; }
    }

    public interface IAutomationWorkflow : IAutomationTask
    {
    }
}
