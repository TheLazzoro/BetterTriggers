using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Components.TriggerEditor.ParameterControls
{
    public interface IValueControl
    {
        Parameter GetSelected();
        int GetElementCount();
    }
}
