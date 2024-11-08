﻿using Joufflu.Popups;
using Joufflu.Shared.Layouts;
using System.Windows.Controls;
using System.Windows.Data;

namespace Automation.App.Components.Inputs
{
    public partial class TextBoxModal : UserControl, IModalContentValidation
    {
        public Modal? ParentLayout { get; set; }
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

        // That's a little bit gadget
        // Used to allow inheritance of TextBoxModal and have a property bind directly on the textbox
        // Allow for INotifyDataErrorInfo handling without too much hassle
        protected void BindValue(string propertyName, object source)
        {
            SubTitle = propertyName;
            Binding valueBinding = new Binding(propertyName);
            valueBinding.Source = source;
            valueBinding.Mode = BindingMode.TwoWay;
            TextBoxValue.SetBinding(TextBox.TextProperty, valueBinding);
        }
    }
}
