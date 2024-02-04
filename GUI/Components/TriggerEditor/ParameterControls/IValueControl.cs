using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public interface IValueControl
    {
        Parameter_Saveable GetSelected();
        event EventHandler SelectionChanged;
        event EventHandler OK;
        int GetElementCount();
        void SetDefaultSelection(Parameter_Saveable parameter);
    }
}
