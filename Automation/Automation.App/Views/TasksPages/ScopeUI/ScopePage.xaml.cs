﻿using Automation.App.Base;
using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Tasks;
using Automation.App.Views.TasksPages.TaskUI;
using Automation.App.Views.TasksPages.WorkflowUI;
using Automation.Shared;
using Joufflu.Shared;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;

namespace Automation.App.Views.TasksPages.ScopeUI
{
    /// <summary>
    /// Logique d'interaction pour ScopePage.xaml
    /// </summary>
    public partial class ScopePage : UserControl, IPage
    {
        public INavigationLayout? Layout { get; set; }
        public Scope Scope { get; set; }

        private IModalContainer _modal => this.GetCurrentModal();

        private readonly App _app = (App)App.Current;
        private readonly ScopeClient _scopeClient;
        private readonly TaskClient _taskClient;

        public ScopePage(Guid scopeId)
        {
            _scopeClient = _app.ServiceProvider.GetRequiredService<ScopeClient>();
            _taskClient = _app.ServiceProvider.GetRequiredService<TaskClient>();
            Scope = new Scope() { Id = scopeId };

            InitializeComponent();
            LoadFullScope(scopeId);
        }

        public async void LoadFullScope(Guid scopeId)
        {
            Scope? fullScope = await _scopeClient.GetByIdAsync(scopeId);

            if (fullScope == null)
                throw new ArgumentException("Scope not found");
            Scope = fullScope;
        }

        #region UI Events
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)sender;
            ContextMenu contextMenu = element.ContextMenu;
            contextMenu.PlacementTarget = element;
            contextMenu.IsOpen = true;
        }

        private async void MenuAddScope_Click(object sender, RoutedEventArgs e)
        {
            Scope newScope = new Scope();
            if (await _modal.Show(new ScopeEditModal(newScope)))
            {
                newScope.Id = await _scopeClient.CreateAsync(newScope);
                Scope.Childrens.Add(newScope);
            }
        }

        private async void MenuAddTask_Click(object sender, RoutedEventArgs e)
        {
            var task = new TaskNode();
            if (await _modal.Show(new TaskEditModal(task)))
            {
                task.Id = await _taskClient.CreateAsync(task);
                Scope.Childrens.Add(task);
            }
        }

        private async void MenuAddWorkflow_Click(object sender, RoutedEventArgs e)
        {
            WorkflowNode workflow = new WorkflowNode();
            if (await _modal.Show(new WorkflowEditModal(workflow)))
            {
                workflow.Id = await _taskClient.CreateAsync(workflow);
                Scope.Childrens.Add(workflow);
            }
        }

        private async void ButtonParameters_Click(object sender, RoutedEventArgs e)
        {
            if (Scope == null)
                return;
            if (await _modal.Show(new ScopeEditModal(Scope)))
                await _scopeClient.UpdateAsync(Scope.Id, Scope);
        }

        #endregion

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            ScopedElement? selectedElement = listBox.SelectedItem as ScopedElement;

            if (selectedElement == null)
                return;

            selectedElement.IsSelected = true;
        }
    }
}
