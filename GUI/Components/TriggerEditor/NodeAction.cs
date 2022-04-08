using Model.Data;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Model.EditorData.Enums;

namespace GUI.Components.TriggerEditor
{
    public class NodeAction : TreeViewItem
    {
        public NodeAction(string text)
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, text, Category.Action);
        }
    }
}
