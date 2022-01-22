using Model.SavableTriggerData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI
{
    public interface IParameterControl
    {
        Parameter GetSelectedItem();
        void SetVisibility(Visibility visibility);
        int GetElementCount();
    }
}
