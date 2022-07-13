using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public interface IValueControl
    {
        Parameter GetSelected();
        event EventHandler SelectionChanged;
        int GetElementCount();
        void SetDefaultSelection(string value);
    }
}
