using GUI.Components.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Components.TriggerEditor
{
    public class CategoryAction : TreeViewItem
    {
        public CategoryAction()
        {
            TreeViewManipulator.SetTreeViewItemAppearance(this, "Action", "Resources/editor-triggeraction.png");
        }
    }
}
