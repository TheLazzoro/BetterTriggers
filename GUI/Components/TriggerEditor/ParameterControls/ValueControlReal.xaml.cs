using BetterTriggers.Models.EditorData;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            if(string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Text = "0.00";
            }
            if (textBox.Text.StartsWith('.'))
            {
                textBox.Text = textBox.Text.Insert(0, "0");
            }

            Value value = new Value()
            {
                value = textBox.Text,
            };

            return value;
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if(!IsTextAllowed(e.Text))
            {
                e.Handled = true;
            }

            previousValue = textBox.Text;
            if (e.Text == ","
                || (e.Text == "." && textBox.Text.Contains("."))
                ||
                (e.Text != "." && e.Text != "," && !int.TryParse(e.Text, out int temp))
                )
            {
                e.Handled = true;
            }
        }

        private void textBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textTemp = textBox.Text;
            if (textTemp.StartsWith('.'))
            {
                textTemp = textTemp.Insert(0, "0");
            }

            bool validNumber = float.TryParse(textTemp, out float temp);
            if (!validNumber)
            {
                if (string.IsNullOrEmpty(textTemp))
                {
                    return;
                }
                else
                {
                    textBox.Text = previousValue;
                }
            }

            previousValue = textBox.Text;
        }

    }
}
