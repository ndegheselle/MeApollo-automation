﻿using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Work;
using Automation.App.Views.WorkPages.Tasks;
using Automation.App.Views.WorkPages.Workflows;
using Automation.Shared.Data;
using Joufflu.Popups;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Usuel.Shared;

namespace Automation.App.Views.WorkPages.Scopes.Components
{
    /// <summary>
    /// Logique d'interaction pour ScopedContextMenu.xaml
    /// </summary>
    public partial class ScopedContextMenu : ContextMenu
    {
        private IModal _modal => this.GetCurrentModalContainer();

        private readonly App _app = (App)App.Current;
        private readonly ScopesClient _scopeClient;
        private readonly TasksClient _taskClient;
        #region Dependency Properties
        // Dependency property Scope RootScope
        public static readonly DependencyProperty RootScopeProperty = DependencyProperty.Register(
            nameof(RootScope),
            typeof(Shared.ViewModels.Work.Scope),
            typeof(ScopedContextMenu),
            new PropertyMetadata(null));

        // Dependency property ScopedElement Selected
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            nameof(Selected),
            typeof(ScopedElement),
            typeof(ScopedContextMenu),
            new PropertyMetadata(null, (o, d) => ((ScopedContextMenu)o).OnSelectedChanged()));
        #endregion

        public Shared.ViewModels.Work.Scope RootScope
        {
            get { return (Shared.ViewModels.Work.Scope)GetValue(RootScopeProperty); }
            set { SetValue(RootScopeProperty, value); }
        }

        public ScopedElement? Selected
        {
            get { return (ScopedElement?)GetValue(SelectedProperty); }
            set { SetValue(SelectedProperty, value); }
        }

        public Shared.ViewModels.Work.Scope CurrentScope => Selected is Shared.ViewModels.Work.Scope scope ? scope : Selected?.Parent ?? RootScope;

        public ICustomCommand RemoveSelectedCommand { get; set; }

        public ICommand AddScopeCommand { get; set; }

        public ICommand AddTaskCommand { get; set; }

        public ICommand AddWorkflowCommand { get; set; }

        public ScopedContextMenu()
        {
            _scopeClient = _app.ServiceProvider.GetRequiredService<ScopesClient>();
            _taskClient = _app.ServiceProvider.GetRequiredService<TasksClient>();

            RemoveSelectedCommand = new DelegateCommand(
                OnRemoveSelected,
                // Not the root element
                () => CurrentScope?.Parent != null);
            AddScopeCommand = new DelegateCommand(OnAddScope);
            AddTaskCommand = new DelegateCommand(OnAddTask);
            AddWorkflowCommand = new DelegateCommand(OnAddWorkflow);
            InitializeComponent();
        }

        private void OnSelectedChanged()
        {
            RemoveSelectedCommand.RaiseCanExecuteChanged();
        }

        private async void OnRemoveSelected()
        {
            if (Selected == null)
                return;

            if (AdonisUI.Controls.MessageBox
                    .Show(
                        "Are you sure you want to remove this element ?",
                        "Confirmation",
                        AdonisUI.Controls.MessageBoxButton.YesNo) !=
                AdonisUI.Controls.MessageBoxResult.Yes)
                return;

            switch (Selected.Type)
            {
                case EnumScopedType.Workflow:
                case EnumScopedType.Task:
                    await _taskClient.DeleteAsync(Selected.Id);
                    break;
                case EnumScopedType.Scope:
                    await _scopeClient.DeleteAsync(Selected.Id);
                    break;
            }
            Selected.Parent!.Childrens.Remove(Selected);
        }

        private async void OnAddScope()
        {
            Shared.ViewModels.Work.Scope newScope = new Shared.ViewModels.Work.Scope();
            newScope.ChangeParent(CurrentScope);
            if (await _modal.Show(new ScopeCreateModal(newScope)))
            {
                Dispatcher.Invoke(
                    () =>
                    {
                        CurrentScope.AddChild(newScope);
                        newScope.FocusOn = EnumScopedTabs.Settings;
                        newScope.IsSelected = true;
                    });
            }
        }

        private async void OnAddTask()
        {
            var task = new TaskNode();
            task.ChangeParent(CurrentScope);
            if (await _modal.Show(new TaskCreateModal(task)))
            {
                CurrentScope.AddChild(task);
                task.FocusOn = EnumScopedTabs.Settings;
                task.IsSelected = true;
            }
        }

        private async void OnAddWorkflow()
        {
            WorkflowNode workflow = new WorkflowNode();
            workflow.ChangeParent(CurrentScope);
            if (await _modal.Show(new WorkflowEditModal(workflow)))
            {
                workflow.Id = await _taskClient.CreateAsync(workflow);
                CurrentScope.AddChild(workflow);
            }
        }
    }
}
