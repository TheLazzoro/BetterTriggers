using GUI.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class Variable : TriggerExplorerElement, ITriggerExplorerElement
    {
        string identifier;
        public bool IsEnabled;
        VariableControl variableControl;

        public Variable(string name, TreeViewItem treeViewItem, VariableControl variableControl) : base(treeViewItem)
        {
            this.IsEnabled = true;
            this.variableControl = variableControl;

            this.Name = name;
            SetName(name);

            ContainerVariables.AddTriggerElement(this);
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
            if (currentTriggerElement != null)
                currentTriggerElement.Hide();

            this.Show();

            currentTriggerElement = this;
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
            this.Name = name;
            this.treeViewItem.Header = name;
            this.variableControl.textBoxVariableName.Text = name;

            var newIdentifier = "udg_" + name;
            this.identifier = newIdentifier;
            this.variableControl.textBlockVariableNameUDG.Text = newIdentifier;
        }
    }
}
