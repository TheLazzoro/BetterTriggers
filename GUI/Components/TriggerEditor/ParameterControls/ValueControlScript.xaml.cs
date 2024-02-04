using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using GUI.Utility;
using Xceed.Wpf.Toolkit;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlScript : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlScript()
        {
            InitializeComponent();

            this.Loaded += ValueControlScript_Loaded;
            this.KeyDown += ValueControlScript_KeyDown;
        }


        private void ValueControlScript_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        private void ValueControlScript_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(Parameter_Saveable parameter)
        {
            textBoxString.Text = parameter.value;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter_Saveable GetSelected()
        {
            Value_Saveable value = new Value_Saveable()
            {
                value = textBoxString.Text,
            };

            return value;
        }

        private void textBoxString_TextChanged(object sender, TextChangedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }
    }
}
