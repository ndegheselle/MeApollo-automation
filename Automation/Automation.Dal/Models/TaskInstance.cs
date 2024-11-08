﻿using Automation.Plugins.Shared;
using Automation.Shared.Data;
using MongoDB.Bson.Serialization.Attributes;

namespace Automation.Dal.Models
{
    public class TaskInstance : ITaskInstance
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid TaskId { get; set; }
        public TargetedPackage Target { get; set; }
        public string? WorkerId { get; set; }

        public TaskContext Context { get; set; }
        public Dictionary<string, object>? Results { get; set; }

        public EnumTaskState State { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TaskInstance(Guid taskId, TargetedPackage target, TaskContext context)
        {
            TaskId = taskId;
            Target = target;
            Context = context;
        }
    }
}
