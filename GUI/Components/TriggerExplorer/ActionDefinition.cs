using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ActionDefinition : IExplorerElement
    {
        public bool IsEnabled;

        public ActionDefinition(string name, TreeViewItem treeViewItem)
        {
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

        public void OnElementClick()
        {
            throw new NotImplementedException();
        }
    }
}
