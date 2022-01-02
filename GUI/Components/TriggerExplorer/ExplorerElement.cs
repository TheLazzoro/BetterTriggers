using GUI.Utility;
using Model.Data;
using System;
using System.Collections.Generic;
using System.IO;
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

            EnumCategory category;
            switch (Path.GetExtension(filePath))
            {

                case ".json":
                    category = EnumCategory.Trigger;
                    break;
                case ".j":
                    category = EnumCategory.AI;
                    break;
                default:
                    category = EnumCategory.Trigger;
                    break;
            }

            TreeViewManipulator.SetTreeViewItemAppearance(this, Path.GetFileNameWithoutExtension(filePath), category);
        }

    }
}
