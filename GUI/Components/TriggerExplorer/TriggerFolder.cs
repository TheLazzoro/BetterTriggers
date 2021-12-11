using GUI.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class TriggerFolder : TriggerExplorerElement, ITriggerExplorerElement
    {
        public TriggerFolder(string name, TreeViewItem treeViewItem) : base(treeViewItem)
        {
            this.Name = name;

            ContainerFolders.AddTriggerElement(this);
        }
        
        public string GetScript()
        {
            throw new NotImplementedException();
        }


        public void OnElementClick()
        {
            if (currentTriggerElement != null)
                currentTriggerElement.Hide();

            this.Show();

            currentTriggerElement = this;
        }

        public void Show() { }
        public void Hide() { }
        public string GetSaveString() { throw new NotImplementedException(); }
        public UserControl GetControl() { throw new NotImplementedException(); }
    }
}
