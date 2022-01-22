using Model.Data;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Model.Enums;

namespace GUI.Components.TriggerEditor
{
    public class NodeAction : TreeViewItem
    {
        public NodeAction()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Action", Category.Action);
        }
    }
}
