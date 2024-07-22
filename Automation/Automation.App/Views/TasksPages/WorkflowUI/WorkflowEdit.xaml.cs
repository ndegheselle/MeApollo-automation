﻿using Automation.App.Base;
using Automation.App.ViewModels.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Automation.App.Views.TasksPages.WorkflowUI
{
    public class WorkflowEditModal : WorkflowEdit, IModalContent
    {
        public IModalContainer? ModalParent { get; set; }
        public ModalOptions Options => new ModalOptions() { Title = "Edit workflow", ValidButtonText = "Save" };

        public WorkflowEditModal(WorkflowScopedItem workflow) : base(workflow)
        {
            if (Workflow.WorkflowNode.Id == Guid.Empty)
                Options.Title = "New workflow";
        }
    }

    /// <summary>
    /// Logique d'interaction pour WorkflowEdit.xaml
    /// </summary>
    public partial class WorkflowEdit : UserControl
    {
        public static readonly DependencyProperty WorkflowProperty =
            DependencyProperty.Register(nameof(Workflow), typeof(WorkflowScopedItem), typeof(WorkflowEdit), new PropertyMetadata(null));

        public WorkflowScopedItem Workflow
        {
            get { return (WorkflowScopedItem)GetValue(WorkflowProperty); }
            set { SetValue(WorkflowProperty, value); }
        }

        public WorkflowEdit()
        {
            InitializeComponent();
        }

        public WorkflowEdit(WorkflowScopedItem workflow)
        {
            Workflow = workflow;
            InitializeComponent();
        }
    }
}
