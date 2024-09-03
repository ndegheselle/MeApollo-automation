﻿using Automation.App.Base;
using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Tasks;
using Automation.Shared;
using Joufflu.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Automation.App.Views.TasksPages.TaskUI
{
    /// <summary>
    /// Logique d'interaction pour TaskPage.xaml
    /// </summary>
    public partial class TaskPage : UserControl, IPage
    {
        public INavigationLayout? Layout { get; set; }
        public TaskNode Task { get; set; }

        private readonly App _app = (App)App.Current;
        private readonly TaskClient _client;
        private IModalContainer _modal => this.GetCurrentModalContainer();

        public TaskPage(Guid taskId)
        {
            _client = _app.ServiceProvider.GetRequiredService<TaskClient>();
            Task = new TaskNode() { Id = taskId };
            InitializeComponent();
            LoadFullTask(taskId);
        }

        public async void LoadFullTask(Guid taskId)
        {
            TaskNode? fullTask = await _client.GetByIdAsync(taskId);

            if (fullTask == null)
                throw new ArgumentException("Task not found");
            Task = fullTask;
        }

        #region UI Events
        private async void ButtonParameters_Click(object sender, RoutedEventArgs e)
        {
            if (Task == null)
                return;
            if (await _modal.Show(new TaskEditModal(Task)))
                await _client.UpdateAsync(Task.Id, Task);
        }
        #endregion
    }
}
