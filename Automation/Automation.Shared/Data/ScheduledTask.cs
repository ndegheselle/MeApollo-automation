﻿using System.Text.Json.Serialization;

namespace Automation.Shared.Data
{
    public enum EnumInstanceStatus
    {
        Pending,
        Running,
        Stopped,
        Completed,
        Failed
    }

    // TODO : Inputs and outputs log
    public class TaskHistory
    {
        public Guid Id { get; set; }
        public Guid? ParentTaskId { get; set; }
        public Guid TaskId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public EnumInstanceStatus Status { get; set; }
    }

    public class ScheduledTask
    {
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }

        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? Day { get; set; }
        public int? Hour { get; set; }
        public int? Minute { get; set; }
        public int? DayOfWeek { get; set; }
    }
}