﻿using Automation.Shared.Data;
using Automation.Shared.Packages;
using System.ComponentModel;
using System.Windows;

namespace Automation.App.Shared.ViewModels.Tasks
{
    public class TaskNode : ScopedElement, INotifyPropertyChanged
    {
        public TargetedPackage? Package { get; set; }
        public Guid ScopeId { get; set; }
        public List<TaskConnector> Inputs { get; set; } = new List<TaskConnector>();
        public List<TaskConnector> Outputs { get; set; } = new List<TaskConnector>();

        public TaskNode()
        {
            Type = EnumScopedType.Task;
        }
    }

    public class TaskConnector : ITaskConnector
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ParentId { get; set; }
        public EnumTaskConnectorType Type { get; set; }
        public EnumTaskConnectorDirection Direction { get; set; }

        // Ui specifics
        public TaskNode Parent { get; set; }
        public bool IsConnected { get; set; }
        public Point Anchor { get; set; }
    }
}
