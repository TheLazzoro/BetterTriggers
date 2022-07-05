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

        public ValueControlInteger()
        {
            InitializeComponent();
            
            textBox.Text = "0";

            this.Loaded += ValueControlInteger_Loaded;
        }

        private void ValueControlInteger_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(string identifier)
        {
            textBox.Text = identifier;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter GetSelected()
        {
            Value value = new Value()
            {
                identifier = textBox.Text,
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
