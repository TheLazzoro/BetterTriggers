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
            ExplorerElement.currentExplorerElement = this;
        }

        public string GetSaveString() { throw new NotImplementedException(); }
        public UserControl GetControl() { throw new NotImplementedException(); }

        public void OnElementRename(string name)
        {
            throw new NotImplementedException();
        }
    }
}
