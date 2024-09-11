﻿using Automation.App.Base;
using Automation.App.Shared.ApiClients;
using Automation.Shared.Base;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Windows.Controls;

namespace Automation.App.Views.PackagesPages.Components
{
    public class PackageSelectorModal : PackageSelector, IModalContent
    {
        public IModalContainer? ModalContainer { get; set; }
        public ModalOptions Options => new ModalOptions() { Title = "Select package", ValidButtonText = "Select" };
    }

    /// <summary>
    /// Logique d'interaction pour PackageSelector.xaml
    /// </summary>
    public partial class PackageSelector : UserControl, INotifyPropertyChanged
    {
        private readonly App _app = (App)App.Current;
        private readonly PackagesClient _packageClient;

        public ListPageWrapper<Package> Packages { get; private set; } = new ListPageWrapper<Package>()
        {
            PageSize = 50,
            Page = 1,
            Total = -1,
        };

        public Package? SelectedPackage { get; set; }
        public string SearchText { get; set; } = string.Empty;

        public PackageSelector()
        {
            _packageClient = _app.ServiceProvider.GetRequiredService<PackagesClient>();
            this.PropertyChanged += PackageSelector_PropertyChanged;
            InitializeComponent();

            Search("");
        }


        private async void Search(string search, int page = 1, int pageSize = 50)
        {
            Packages = await _packageClient.SearchAsync(search, page, pageSize);
        }

        #region UI events
        private void PackageSelector_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SearchText))
            {
                Search(SearchText.Trim(), InstancesPaging.PageNumber, InstancesPaging.Capacity);
            }
        }
        
        private void InstancesPaging_PagingChange(int pageNumber, int capacity)
        {
            Search(SearchText.Trim(), pageNumber, capacity);
        }
        #endregion
    }
}
