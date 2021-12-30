using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Components.TriggerExplorer
{
    public class ExplorerElement : TreeViewItem
    {
        public string FilePath;
        public TabItem tabItem;
        public IExplorerElement Ielement;
        public static IExplorerElement currentExplorerElement;

        public ExplorerElement(string filePath)
        {
            this.FilePath = filePath;
        }

    }
}
