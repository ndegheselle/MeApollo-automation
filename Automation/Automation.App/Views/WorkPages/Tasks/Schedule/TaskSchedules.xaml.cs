﻿using Automation.App.Shared.ViewModels.Work;
using Joufflu.Popups;
using System.Windows;
using System.Windows.Controls;

namespace Automation.App.Views.WorkPages.Tasks.Schedule
{
    /// <summary>
    /// Logique d'interaction pour TaskSchedule.xaml
    /// </summary>
    public partial class TaskSchedules : UserControl
    {
        public static readonly DependencyProperty TaskProperty = DependencyProperty.Register(
            nameof(Task),
            typeof(TaskNode),
            typeof(TaskSchedules),
            new PropertyMetadata(default(TaskNode), (o, e) => ((TaskSchedules)o).OnTargetChange()));

        public TaskNode Task { get { return (TaskNode)GetValue(TaskProperty); } set { SetValue(TaskProperty, value); } }
        private IModal _modal => this.GetCurrentModalContainer();

        public TaskSchedules() { InitializeComponent(); }

        private void OnTargetChange()
        {
            // TODO load task schedule
        }
    }
}
