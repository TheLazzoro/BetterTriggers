using DataAccess.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class TriggerEvent : TriggerElement
    {
        private DataAccess.Natives.Event _event;
        
        public TriggerEvent(DataAccess.Natives.Event _event)
        {
            this._event = _event;
            this.parameters = _event.parameters;
            this.paramText = _event.eventText;

            TreeViewManipulator.SetTreeViewItemAppearance(this, "Action", _event.category);
            this.FormatParameterText(paramTextBlock);
        }
    }
}
