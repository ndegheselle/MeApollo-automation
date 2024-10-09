﻿using Automation.App.Views.PackagesPages.Components;
using Automation.Shared.Data;
using Joufflu.Popups;
using Joufflu.Shared.Layouts;
using System.Windows.Controls;

namespace Automation.App.Views.PackagesPages
{
    /// <summary>
    /// Logique d'interaction pour PackagesMainPage.xaml
    /// </summary>
    public partial class PackagesMainPage : UserControl, IPage
    {
        private IModal _modal => this.GetCurrentModalContainer();
        public PackagesMainPage()
        {
            InitializeComponent();
        }
    }
}
