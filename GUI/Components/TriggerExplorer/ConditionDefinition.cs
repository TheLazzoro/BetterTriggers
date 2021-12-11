using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ConditionDefinition : TriggerExplorerElement, ITriggerExplorerElement
    {
        public bool IsEnabled;

        public ConditionDefinition(string name, TreeViewItem treeViewItem) : base(treeViewItem)
        {
            this.Name = name;
            this.IsEnabled = true;
        }

        public UserControl GetControl()
        {
            throw new NotImplementedException();
        }

        public string GetSaveString()
        {
            throw new NotImplementedException();
        }

        public string GetScript()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void OnElementClick()
        {
            throw new NotImplementedException();
        }

        public void Show()
        {
            throw new NotImplementedException();
        }
    }
}
