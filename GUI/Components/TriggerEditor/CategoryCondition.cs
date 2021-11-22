using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public class CategoryCondition : TreeViewItem
    {
        public CategoryCondition()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Condition", "Resources/editor-triggercondition.png");
        }
    }
}
