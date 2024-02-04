using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public interface IParameterControl
    {
        Parameter_Saveable GetSelectedItem();
        void SetVisibility(Visibility visibility);
        int GetElementCount();
        void SetDefaultSelection(Parameter_Saveable parameter);
    }
}
