﻿using Joufflu.Popups;
using System.Windows.Controls;
using System.Windows.Data;

namespace Automation.App.Components.Inputs
{
    public partial class TextBoxModal : UserControl, IModalValidationContent
    {
        public ModalValidationOptions Options { get; private set; } = new ModalValidationOptions()
        {
            Title = "Input data",
            ValidButtonText = "Ok",
        };
        public string SubTitle { get; set; } = string.Empty;

        public TextBoxModal(string titre)
        {
            Options.Title = titre;
            InitializeComponent();
        }

        protected void BindValue(string propertyName, object source)
        {
            SubTitle = propertyName;
            Binding valueBinding = new Binding(propertyName);
            valueBinding.Source = source;
            valueBinding.Mode = BindingMode.TwoWay;
            TextBoxValue.SetBinding(TextBox.TextProperty, valueBinding);
        }

        public Task<bool> OnValidation()
        {
            return Task.FromResult(true);
        }
    }
}
