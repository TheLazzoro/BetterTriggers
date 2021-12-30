using GUI.Containers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class TriggerFolder : IExplorerElement
    {
        public TriggerFolder()
        {
        }
        
        public string GetScript()
        {
            throw new NotImplementedException();
        }


        public void OnElementClick()
        {
            if (ExplorerElement.currentExplorerElement != null)
                ExplorerElement.currentExplorerElement.Hide();

            this.Show();

            ExplorerElement.currentExplorerElement = this;
        }

        public void Show() { }
        public void Hide() { }
        public string GetSaveString() { throw new NotImplementedException(); }
        public UserControl GetControl() { throw new NotImplementedException(); }
    }
}
