using GUI.Utility;
using Model.Data;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GUI.Components.TriggerExplorer
{
    public class ExplorerElement : TreeViewItem
    {
        public string FilePath;
        public string ElementName;
        public TabItemBT tabItem;
        public IExplorerElement Ielement;
        public static IExplorerElement currentExplorerElement;

        public ExplorerElement(string filePath)
        {
            this.FilePath = filePath;
            this.ElementName = Path.GetFileNameWithoutExtension(filePath);

            RefreshElement();
        }

        public ExplorerElement(string rootPath, bool isRoot)
        {
            this.FilePath = rootPath;
            this.ElementName = Path.GetFileNameWithoutExtension(rootPath);
            TreeViewManipulator.SetTreeViewItemAppearance(this, ElementName, Category.Map);
        }

        public void RefreshElement()
        {
            Category category;
            switch (Path.GetExtension(this.FilePath))
            {
                case "":
                    category = Category.Folder;
                    break;
                case ".trg":
                    category = Category.Trigger;
                    break;
                case ".j":
                    category = Category.AI;
                    break;
                case ".var":
                    category = Category.SetVariable;
                    break;
                default:
                    category = Category.Trigger;
                    break;
            }

            TreeViewManipulator.SetTreeViewItemAppearance(this, ElementName, category);
            
            if(this.tabItem != null)
                tabItem.RefreshHeader(ElementName);

            if(Ielement != null)
                Ielement.OnElementRename(ElementName);
        }
    }
}
