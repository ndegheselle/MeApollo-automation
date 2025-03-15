﻿using Automation.Shared.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Automation.Dal.Models
{
    public class TaskConnectors : IAutomationTaskConnector
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public EnumTaskConnectorType Type { get; set; }
        public EnumTaskConnectorDirection Direction { get; set; }
    }

    [JsonDerivedType(typeof(AutomationWorkflow), "workflow")]
    [BsonKnownTypes(typeof(AutomationWorkflow))]
    public class AutomationTask : ScopedElement, IAutomationTask
    {
        public IEnumerable<IAutomationTaskConnector> Inputs { get; set; } = [];
        public IEnumerable<IAutomationTaskConnector> Outputs { get; set; } = [];
        public IEnumerable<Schedule> Schedules { get; set; } = [];
        public TargetedPackage? Package { get; set; }

        public AutomationTask() {
            Type = EnumScopedType.Task;
        }
    }

    public class AutomationWorkflow : AutomationTask, IAutomationWorkflow
    {
        public AutomationWorkflow()
        {
            Type = EnumScopedType.Workflow;
        }
    }
}
