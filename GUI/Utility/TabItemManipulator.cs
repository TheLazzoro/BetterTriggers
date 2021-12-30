using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace GUI.Utility
{
    public static class TabItemManipulator
    {
        public static TabItem SetTabItemApperance(object content, string name)
        {
            TabItem tabItem = new TabItem();
            tabItem.Content = content;
            tabItem.Header = name;

            return tabItem;
        }
    }
}
