using Model.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public class NodeEvent : TreeViewItem
    {
        public NodeEvent()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Events", EnumCategory.Event);
        }
    }
}
