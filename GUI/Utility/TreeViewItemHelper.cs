using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows;

namespace GUI.Utility
{
    public static class TreeViewItemHelper
    {
        /// <summary>
        /// // It is necessary to traverse the item's parents since drag & drop picks up
        /// things like 'TextBlock' and 'Border' on the drop target when dropping the 
        /// dragged element.
        /// </summary>
        /// <returns></returns>
        public static TreeViewItem GetTraversedTargetDropItem(DependencyObject dropTarget)
        {
            if (dropTarget == null || dropTarget is TreeView)
                return null;

            TreeViewItem traversedTarget = null;
            while (traversedTarget == null)
            {
                if (dropTarget is not TextElement)
                    dropTarget = VisualTreeHelper.GetParent(dropTarget);
                else
                {
                    var d = (TextElement)dropTarget;
                    dropTarget = d.Parent;
                }


                if (dropTarget is TreeViewItem)
                {
                    traversedTarget = (TreeViewItem)dropTarget;
                }

                if (dropTarget == null)
                    return null;
            }

            return traversedTarget;
        }


        public static ItemsControl GetTreeItemParent(TreeViewItem item)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(item);
            while (!(parent is TreeViewItem || parent is TreeView))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }
            return parent as ItemsControl;
        }
    }
}
