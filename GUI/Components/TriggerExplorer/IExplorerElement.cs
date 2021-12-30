using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public interface IExplorerElement
    {
        string GetSaveString();
        string GetScript();
        void OnElementClick();
        UserControl GetControl();
        void Show();
        void Hide();
    }
}
