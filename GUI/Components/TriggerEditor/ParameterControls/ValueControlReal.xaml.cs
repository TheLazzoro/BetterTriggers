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
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Controllers;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlReal : UserControl, IValueControl
    {
        private string previousValue;

        public event EventHandler SelectionChanged;
        public event EventHandler OK;

        public ValueControlReal()
        {
            InitializeComponent();
            
            textBox.Text = "0.00";

            this.Loaded += ValueControlReal_Loaded;
            this.KeyDown += ValueControlReal_KeyDown;
        }


        private void ValueControlReal_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
            textBox.Focus();
            textBox.SelectAll();
        }

        private void ValueControlReal_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            EventHandler handler = OK;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(Parameter parameter)
        {
            textBox.Text = parameter.value;
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
            if (e.Text == "," || (e.Text == "." && textBox.Text.Contains(".")))
            {
                e.Handled = true;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                float value = float.Parse(textBox.Text);
            }
            catch (Exception ex)
            {
                textBox.Text = previousValue;
            }

            previousValue = textBox.Text;
        }
    }
}
