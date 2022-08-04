using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
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
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Controllers;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlInteger : UserControl, IValueControl
    {
        private string previousValue;

        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlInteger()
        {
            InitializeComponent();
            
            textBox.Text = "0";

            this.Loaded += ValueControlInteger_Loaded;
            this.KeyDown += ValueControlInteger_KeyDown;
        }

        private void ValueControlInteger_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        private void ValueControlInteger_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
            textBox.Focus();
            textBox.SelectAll();
        }

        public void SetDefaultSelection(string value)
        {
            textBox.Text = value;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter GetSelected()
        {
            Value value = new Value()
            {
                value = textBox.Text,
            };

            return value;
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            previousValue = textBox.Text;
            
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                int value = int.Parse(textBox.Text);
            }
            catch (Exception ex)
            {
                textBox.Text = previousValue;
            }

            previousValue = textBox.Text;
        }
    }
}
