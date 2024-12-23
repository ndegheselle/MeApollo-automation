﻿using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Work;
using Joufflu.Popups;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Windows.Controls;

namespace Automation.App.Views.WorkPages.Tasks.Instances
{
    /// <summary>
    /// Logique d'interaction pour TaskExecute.xaml
    /// </summary>
    public partial class TaskExecuteModal : UserControl, IModalContentValidation
    {
        private readonly App _app = (App)App.Current;
        private readonly TasksClient _taskClient;

        public Modal? ParentLayout { get; set; }
        public ModalOptions Options { get; private set; } = new ModalOptions() { Title = "Execute task" };

        public string JsonSettings { get; set; } = "";
        public TaskNode Task { get; set; }

        public TaskExecuteModal(TaskNode task) {
            _taskClient = _app.ServiceProvider.GetRequiredService<TasksClient>();
            Task = task;
            Options.Title = $"Execute task - {task.Name}";
            InitializeComponent(); 
        }

        public async Task<bool> OnValidation()
        {
            TaskInstance instance = await _taskClient.ExecuteAsync(Task.Id, JsonSerializer.Deserialize<object>(JsonSettings));
            ParentLayout?.Show(new TaskProgressionModal(instance));

            return true;
        }
    }
}