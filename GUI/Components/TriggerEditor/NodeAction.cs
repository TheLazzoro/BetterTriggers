using Model.Data;
using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public class NodeAction : TreeViewItem
    {
        public NodeAction()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Action", EnumCategory.Action);
        }
    }
}
