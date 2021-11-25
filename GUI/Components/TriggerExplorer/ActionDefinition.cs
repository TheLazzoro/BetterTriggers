using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ActionDefinition : TriggerExplorerElement, ITriggerExplorerElement
    {
        public bool IsEnabled;

        public ActionDefinition(string name, TreeViewItem treeViewItem) : base(treeViewItem)
        {
            this.Name = name;
            this.IsEnabled = true;
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
