using GUI.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class Variable : IExplorerElement
    {
        public bool IsEnabled;
        string identifier;
        VariableControl variableControl;

        public Variable(VariableControl variableControl)
        {
            this.IsEnabled = true;
            this.variableControl = variableControl;

            ContainerITriggerElements.AddTriggerElement(this);

            // Events in the variableControl
            variableControl.OnRename += delegate
            {
                SetName(variableControl.textBoxVariableName.Text);
            };
        }

        public void Hide()
        {
            variableControl.Visibility = Visibility.Hidden;
        }

        public void OnElementClick()
        {
            if (ExplorerElement.currentExplorerElement != null)
                ExplorerElement.currentExplorerElement.Hide();

            this.Show();

            ExplorerElement.currentExplorerElement = this;
        }

        public void Show()
        {
            variableControl.Visibility = Visibility.Visible;
        }

        public string GetScript()
        {
            return $"globals\ninteger {this.identifier}\nendglobals";
        }

        private void SetName(string name)
        {
            this.variableControl.textBoxVariableName.Text = name;

            var newIdentifier = "udg_" + name;
            this.identifier = newIdentifier;
            this.variableControl.textBlockVariableNameUDG.Text = newIdentifier;
        }

        public string GetSaveString()
        {
            throw new NotImplementedException();
        }

        public UserControl GetControl()
        {
            return this.variableControl;
        }
    }
}
