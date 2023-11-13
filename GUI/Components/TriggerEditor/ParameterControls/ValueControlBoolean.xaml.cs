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
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlBoolean : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlBoolean()
        {
            InitializeComponent();

            checkBox.Click += CheckBox_Click;
            this.KeyDown += ValueControlBoolean_KeyDown;
        }


        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        private void ValueControlBoolean_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public int GetElementCount()
        {
            return 1;
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            checkBox.IsChecked = parameter.value == "true";
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
                value = str,
            };

            return value;
        }
    }
}
