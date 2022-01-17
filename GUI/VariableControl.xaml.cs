using GUI.Components.TriggerExplorer;
using GUI.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for VariableControl.xaml
    /// </summary>
    public partial class VariableControl : UserControl, IExplorerElement
    {
        public VariableControl()
        {
            InitializeComponent();

            // Events in the variableControl
            this.OnRename += delegate
            {
                //OnElementRename(textBoxVariableName.Text);
            };
        }

        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void OnElementClick()
        {
            ExplorerElement.currentExplorerElement = this;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        public void OnElementRename(string name)
        {
            var newIdentifier = "udg_" + name;
            this.textBlockVariableNameUDG.Text = newIdentifier;
        }

        public string GetSaveString()
        {
            throw new NotImplementedException();
        }

        public UserControl GetControl()
        {
            return this;
        }

        [Browsable(true)]
        [Category("Action")]
        public event EventHandler OnRename;

        private void textBoxVariableName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                //bubble the event up to the parent
                if (this.OnRename != null)
                    this.OnRename(this, e);
            }
        }
    }
}
