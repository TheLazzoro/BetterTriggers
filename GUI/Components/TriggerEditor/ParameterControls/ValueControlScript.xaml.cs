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
using BetterTriggers.Controllers;
using BetterTriggers.WorldEdit;
using GUI.Controllers;
using GUI.Utility;
using Model;
using Model.SaveableData;
using Model.Templates;
using Xceed.Wpf.Toolkit;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public partial class ValueControlScript : UserControl, IValueControl
    {
        public event EventHandler SelectionChanged;
        public ValueControlScript()
        {
            InitializeComponent();

            this.Loaded += ValueControlScript_Loaded;
        }

        private void ValueControlScript_Loaded(object sender, RoutedEventArgs e)
        {
            EventHandler handler = SelectionChanged;
            handler?.Invoke(this, e);
        }

        public void SetDefaultSelection(string identifier)
        {
            textBoxString.Text = identifier;
        }

        public int GetElementCount()
        {
            return 1;
        }

        public Parameter GetSelected()
        {
            Value value = new Value()
            {
                identifier = textBoxString.Text,
                returnType = "scriptcode"
            };

            return value;
        }
    }
}
