﻿using Automation.Shared.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Automation.Dal.Models
{
    /// <summary>
    /// This class is used only on the DB side
    /// </summary>
    public class InstanceContext
    {
        public string? Settings { get; set; }
    }

    public class TaskInstance : ITaskInstance
    {
        [BsonId]
        public Guid Id { get; set; }

        public Guid TaskId { get; set; }
        public TargetedPackage Target { get; set; }
        public string? WorkerId { get; set; }

        [JsonIgnore]
        public InstanceContext Context { get; set; }
        [JsonIgnore]
        public Dictionary<string, object>? Results { get; set; }

        public EnumTaskState State { get; set; }

        public DateTime CreateDate { get; set; } = DateTime.Now;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public TaskInstance(Guid taskId, TargetedPackage target, InstanceContext context)
        {
            TaskId = taskId;
            Target = target;
            Context = context;
        }
    }
}
