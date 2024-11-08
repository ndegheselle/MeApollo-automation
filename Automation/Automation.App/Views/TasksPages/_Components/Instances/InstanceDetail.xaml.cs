﻿using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Tasks;
using Joufflu.Popups;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Text.Json;
using System.Windows.Controls;

namespace Automation.App.Views.TasksPages.Components.Instances
{
    /// <summary>
    /// Logique d'interaction pour InstanceDetail.xaml
    /// </summary>
    public partial class InstanceDetail : UserControl, IModalContent, INotifyPropertyChanged
    {
        public Modal? ParentLayout { get; set; }
        public ModalOptions? Options => new ModalOptions() { Title = "Instance detail" };
        public TaskInstance Instance { get; private set; }

        public string ContextJson { get; set; } = "";
        public string ResultJson { get; set; } = "";

        private readonly App _app = (App)App.Current;
        private readonly TaskInstancesClient _instanceClient;

        public InstanceDetail(TaskInstance instance)
        {
            _instanceClient = _app.ServiceProvider.GetRequiredService<TaskInstancesClient>();
            Instance = instance;
            InitializeComponent();
            LoadFullInstance(Instance.Id);
        }

        private async void LoadFullInstance(Guid instanceId)
        {
            Instance = await _instanceClient.GetByIdAsync(instanceId) ?? throw new ArgumentException("Instance not found");
            ContextJson = JsonSerializer.Serialize(Instance.Context, new JsonSerializerOptions() { WriteIndented = true });
            ResultJson = JsonSerializer.Serialize(Instance.Results, new JsonSerializerOptions() { WriteIndented = true });
        }
    }
}
