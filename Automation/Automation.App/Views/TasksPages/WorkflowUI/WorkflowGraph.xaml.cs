﻿using Automation.App.Base;
using Automation.App.Components.Display;
using Automation.App.ViewModels.Tasks;
using Automation.App.Views.TasksPages.Components;
using Automation.Supervisor.Client;
using Microsoft.Extensions.DependencyInjection;
using Nodify;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

using Point = System.Drawing.Point;

namespace Automation.App.Views.TasksPages.WorkflowUI
{
    /// <summary>
    /// Logique d'interaction pour WorkflowGraph.xaml
    /// </summary>
    public partial class WorkflowGraph : UserControl
    {
        #region Dependency Properties

        // Dependency property Editor of type EditorViewModel
        public static readonly DependencyProperty EditorDataProperty = DependencyProperty.Register(
            "EditorData",
            typeof(EditorViewModel),
            typeof(WorkflowGraph),
            new PropertyMetadata(null, (o, e) => ((WorkflowGraph)o).OnEditorDataChange()));

        public EditorViewModel EditorData
        {
            get => (EditorViewModel)GetValue(EditorDataProperty);
            set => SetValue(EditorDataProperty, value);
        }

        private void OnEditorDataChange() { EditorData.InvalidConnection += _alert.Warning; }
        #endregion

        private readonly App _app = (App)App.Current;
        private readonly IModalContainer _modal;
        private readonly IAlert _alert;
        private readonly ITaskClient _nodeClient;
        private readonly IScopeClient _scopeClient;

        public WorkflowGraph()
        {
            _nodeClient = _app.ServiceProvider.GetRequiredService<ITaskClient>();
            _scopeClient = _app.ServiceProvider.GetRequiredService<IScopeClient>();
            _modal = _app.ServiceProvider.GetRequiredService<IModalContainer>();
            _alert = _app.ServiceProvider.GetRequiredService<IAlert>();
            InitializeComponent();
        }

        #region UI Events
        private async void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            ScopedSelectorModal nodeSelector = new ScopedSelectorModal()
            {
                RootScope = new ScopeItem(await _scopeClient.GetRootScopeAsync()),
                AllowedSelectedNodes = EnumScopedType.Workflow | EnumScopedType.Task
            };

            if (await _modal.Show(nodeSelector) && nodeSelector.Selected != null)
            {
                ScopedTaskItem taskScopedItem = (ScopedTaskItem)nodeSelector.Selected;
                // TODO : Create the relation through the API
                EditorData.Workflow.Nodes.Add(new NodifyNode(new Shared.Data.WorkflowRelation(), taskScopedItem.TaskNode));
            }
        }

        // Handle suppr key to remove selected node
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedNodes();
            }
        }

        private void ButtonZoomIn_Click(object sender, RoutedEventArgs e) { Editor.ZoomIn(); }

        private void ButtonZoomOut_Click(object sender, RoutedEventArgs e) { Editor.ZoomOut(); }

        private void ButtonZoomFit_Click(object sender, RoutedEventArgs e) { Editor.FitToScreen(); }

        private void ToggleButtonSnapping_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggleButton = (ToggleButton)sender;
            Editor.GridCellSize = toggleButton.IsChecked == true ? EditorViewModel.GRID_DEFAULT_SIZE : 1;
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e) { DeleteSelectedNodes(); }

        private void ButtonGroup_Click(object sender, RoutedEventArgs e)
        {
            Rectangle boundingBox = GetSelectedBoundingBox(10);
            EditorData.CreateGroup(boundingBox);
        }
        #endregion

        private async void DeleteSelectedNodes()
        {
            if (await _modal.Show(
                new ConfirmationModal($"Are you sure you want to delete these {Editor.SelectedItems?.Count} nodes ?")) == false)
                return;

            EditorData.RemoveNodes(EditorData.SelectedNodes);
        }

        private Rectangle GetSelectedBoundingBox(int padding)
        {
            Point min = new Point(int.MaxValue, int.MaxValue);
            Point max = new Point(int.MinValue, int.MinValue);

            if (Editor.SelectedItems == null || Editor.SelectedItems.Count == 0)
                return Rectangle.Empty;

            foreach (var node in Editor.SelectedItems)
            {
                var container = Editor.ItemContainerGenerator.ContainerFromItem(node) as ItemContainer;
                if (container == null)
                    continue;

                min.X = Math.Min(min.X, (int)container.Location.X);
                min.Y = Math.Min(min.Y, (int)container.Location.Y);
                max.X = Math.Max(max.X, (int)container.Location.X + (int)container.ActualSize.Width);
                max.Y = Math.Max(max.Y, (int)container.Location.Y + (int)container.ActualSize.Height);
            }

            return new Rectangle(min.X - padding, min.Y - padding, max.X - min.X + padding * 2, max.Y - min.Y + padding * 2);
        }
    }
}