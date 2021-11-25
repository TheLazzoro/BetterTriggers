using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Components.TriggerExplorer
{
    public interface ITriggerExplorerElement
    {
        string GetScript();
        void OnElementClick();
        void Show();
        void Hide();
    }
}
