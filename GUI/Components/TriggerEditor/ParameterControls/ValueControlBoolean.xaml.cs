using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BetterTriggers.Controllers;
using BetterTriggers.WorldEdit;
using GUI.Controllers;
using Model;
using Model.SaveableData;
using Model.Templates;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlBoolean : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;

        public ValueControlBoolean()
        {
            InitializeComponent();

            checkBox.Click += CheckBox_Click;
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public int GetElementCount()
        {
            return 1;
        }

        public void SetDefaultSelection(string identifier)
        {
            checkBox.IsChecked = identifier == "true";
        }

        public Parameter GetSelected()
        {
            string str = string.Empty;
            if ((bool)checkBox.IsChecked)
                str = "true";
            else
                str = "false";

            Value value = new Value()
            {
                identifier = str,
                returnType = "boolean"
            };

            return value;
        }
    }
}
