using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public interface IExplorerElement
    {
        string GetSaveString();
        void OnElementClick();
        void OnElementRename(string name);
        UserControl GetControl();
    }
}
