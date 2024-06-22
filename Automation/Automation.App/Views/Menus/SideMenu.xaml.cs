﻿using Automation.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace Automation.App.Views.Menus
{
    /// <summary>
    /// Logique d'interaction pour SideMenu.xaml
    /// </summary>
    public partial class SideMenu : UserControl
    {
        private readonly SideMenuViewModel _sideMenuContext;
        private readonly App _app = (App)App.Current;

        public SideMenu()
        {
            _sideMenuContext = _app.ServiceProvider.GetRequiredService<SideMenuViewModel>();
            this.DataContext = _sideMenuContext;
            InitializeComponent();
        }
    }
}
