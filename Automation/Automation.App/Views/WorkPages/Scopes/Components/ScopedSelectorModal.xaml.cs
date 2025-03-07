﻿using Automation.App.Shared.ApiClients;
using Automation.App.Shared.ViewModels.Work;
using Joufflu.Popups;
using Microsoft.Extensions.DependencyInjection;
using System.Transactions;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Usuel.Shared;

namespace Automation.App.Views.WorkPages.Scopes.Components
{
    /// <summary>
    /// Logique d'interaction pour ScopedSelectorModal.xaml
    /// </summary>
    public partial class ScopedSelectorModal : UserControl, IModalContent
    {
        public Modal? ParentLayout { get; set; }

        public ModalOptions Options { get; } = new ModalOptions() { Title = "Add node" };

        public ICustomCommand SelectCommand { get; }
        public ScopedElement? Selected { get; set; }
        public Scope? CurrentScope { get; set; }

        private readonly App _app = App.Current;
        private readonly ScopesClient _client;

        public ScopedSelectorModal()
        {
            _client = _app.ServiceProvider.GetRequiredService<ScopesClient>();
            this.Loaded += ScopedSelectorModal_Loaded;

            SelectCommand = new DelegateCommand(
                () => ParentLayout?.Hide(true),
                () => Selected != null &&
                    (Selected.Type == Automation.Shared.Data.EnumScopedType.Workflow ||
                        Selected.Type == Automation.Shared.Data.EnumScopedType.Task));

            InitializeComponent();
        }

        private async void ScopedSelectorModal_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            CurrentScope = await _client.GetRootAsync();
            CurrentScope.RefreshChildrens();
        }

        private void ListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Selected is Scope scope)
            {
                CurrentScope = scope;
                // TODO : load childrens
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCommand.RaiseCanExecuteChanged();
        }
    }
}
