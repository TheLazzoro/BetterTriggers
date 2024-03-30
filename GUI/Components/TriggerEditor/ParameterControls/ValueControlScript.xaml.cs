using BetterTriggers.Models.EditorData;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        public void SetDefaultSelection(Parameter parameter)
        {
            textBoxString.Text = parameter.value;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter GetSelected()
        {
            Value value = new Value()
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
