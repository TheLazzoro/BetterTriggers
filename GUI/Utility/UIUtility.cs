using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Utility
{
    public enum TreeItemLocation
    {
        Top,
        Middle,
        Bottom,
    }

    public static class UIUtility
    {
        public static bool IsCircularParent(TreeViewItem dragItem, TreeViewItem dropTarget)
        {
            bool IsCircularParent = false;
            DependencyObject parent = dropTarget;
            while (IsCircularParent == false)
            {
                parent = VisualTreeHelper.GetParent(parent);
                if (parent == null)
                    break;

                if (parent == dragItem)
                    IsCircularParent = true;
            }

            return IsCircularParent;
        }

        public static bool IsMouseInFirstHalf(FrameworkElement container, Point mousePosition, Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return mousePosition.Y < container.ActualHeight / 2;
            }
            return mousePosition.X < container.ActualWidth / 2;
        }

        public static TreeItemLocation TreeItemGetMouseLocation(FrameworkElement container, Point mousePosition)
        {
            if (mousePosition.Y < container.RenderSize.Height / 3)
                return TreeItemLocation.Top;
            else if (mousePosition.Y > container.RenderSize.Height * 2 / 3)
                return TreeItemLocation.Bottom;
            else 
                return TreeItemLocation.Middle;
        }
    }
}
