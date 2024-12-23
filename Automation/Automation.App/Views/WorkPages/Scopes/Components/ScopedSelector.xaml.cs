﻿using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Work;
using Automation.Shared.Data;
using Joufflu.Popups;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Automation.App.Views.WorkPages.Scopes.Components
{
    public class ScopedSelectorModal : ScopedSelector, IModalContent
    {
        public Modal? ParentLayout { get; set; }
        public ModalOptions Options => new ModalOptions() { Title = "Add node" };
    }

    /// <summary>
    /// Logique d'interaction pour ScopedElementSelector.xaml
    /// </summary>
    public partial class ScopedSelector : UserControl
    {
        public event Action<ScopedElement?>? SelectedChanged;

        #region Dependency Properties
        // Dependency property Scope RootScope
        public static readonly DependencyProperty RootScopeProperty = DependencyProperty.Register(
            nameof(RootScope),
            typeof(Shared.ViewModels.Work.Scope),
            typeof(ScopedSelector),
            new PropertyMetadata(null));

        // Dependency property ScopedElement Selected
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(
            nameof(Selected),
            typeof(ScopedElement),
            typeof(ScopedSelector),
            new PropertyMetadata(null));
        #endregion

        #region Props
        public Shared.ViewModels.Work.Scope RootScope
        {
            get { return (Shared.ViewModels.Work.Scope)GetValue(RootScopeProperty); }
            set { SetValue(RootScopeProperty, value); }
        }
        public ScopedElement? Selected
        {
            get { return (ScopedElement?)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);
                SelectedChanged?.Invoke(Selected);
            }
        }
        public Shared.ViewModels.Work.Scope CurrentScope => Selected is Shared.ViewModels.Work.Scope scope ? scope : Selected?.Parent ?? RootScope;

        public EnumScopedType AllowedSelectedNodes
        {
            get;
            set;
        } = EnumScopedType.Scope | EnumScopedType.Workflow | EnumScopedType.Task;


        private readonly App _app = (App)App.Current;
        private readonly ScopesClient _scopeClient;
        private readonly TasksClient _taskClient;

        private IModal _modal => this.GetCurrentModalContainer();
        #endregion

        public ScopedSelector()
        {
            _scopeClient = _app.ServiceProvider.GetRequiredService<ScopesClient>();
            _taskClient = _app.ServiceProvider.GetRequiredService<TasksClient>();
            InitializeComponent();
        }

        // XXX : move to viewmodels ?
        #region Commands
        
        #endregion

        #region UI Events
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeView treeView = (TreeView)sender;
            ScopedElement? selected = treeView.SelectedItem as ScopedElement;
            if (selected != null && !AllowedSelectedNodes.HasFlag(selected.Type))
            {
                selected.IsSelected = false;
                return;
            }
            Selected = selected;
        }

        // Allow selection on right click
        private void TreeView_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            TreeViewItem? treeViewItem = VisualUpwardSearch((DependencyObject)e.OriginalSource);

            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
            // Unselect the current if not null
            else if (Selected != null)
            {
                Selected.IsSelected = false;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement frameworkElement = (FrameworkElement)sender;
            frameworkElement.ContextMenu.PlacementTarget = frameworkElement;
            frameworkElement.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            frameworkElement.ContextMenu.IsOpen = true;
        }
        #endregion

        private TreeViewItem? VisualUpwardSearch(DependencyObject source)
        {
            while (source != null && !(source is TreeViewItem))
                source = VisualTreeHelper.GetParent(source);

            return source as TreeViewItem;
        }
    }
}