using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Components.TriggerExplorer
{
    public interface ITriggerElement
    {
        string GetScript();
        void OnElementClick();
        void Show();
        void Hide();
    }
}
