﻿using Automation.Shared.Data;
using System.Windows;
using System.Windows.Controls;

namespace Automation.App.Views.WorkPages.Components
{
    public partial class NodeIcon : UserControl
    {
        public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
            "Type",
            typeof(EnumScopedType),
            typeof(NodeIcon),
            new PropertyMetadata(EnumScopedType.Task));

        public EnumScopedType Type
        {
            get { return (EnumScopedType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        public NodeIcon()
        {
            InitializeComponent();
        }
    }
}
