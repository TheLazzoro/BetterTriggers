using GUI.Components.TriggerExplorer;
using GUI.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerFileSystem
    {
        public void CreateElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);

            // Find matching directory
            var rootNode = triggerExplorer.map;
            ExplorerElement parent = FindTreeNodeDirectory(rootNode, directory);
            if (parent == null)
                parent = rootNode;

            // Create ExplorerElement in the parent node
            string name = Path.GetFileNameWithoutExtension(fullPath);
            ExplorerElement explorerElement = new ExplorerElement(fullPath);
            parent.Items.Insert(parent.Items.Count, explorerElement);
        }

        public void DeleteElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            var rootNode = triggerExplorer.map;
            ExplorerElement elementToDelete = FindTreeNodeElement(rootNode, fullPath);

            if(elementToDelete != null)
            {
                var parent = (ExplorerElement) elementToDelete.Parent;
                parent.Items.Remove(elementToDelete);

                // Remove tab item
                if(elementToDelete.tabItem != null)
                {
                    var tabParent = (TabControl) elementToDelete.tabItem.Parent;
                    tabParent.Items.Remove(elementToDelete.tabItem);
                }
            }
        }

        private ExplorerElement FindTreeNodeElement(ExplorerElement parent, string path)
        {
            ExplorerElement node = null;
            
            for (int i = 0; i < parent.Items.Count; i++)
            {
                ExplorerElement element = parent.Items[i] as ExplorerElement;
                if(element.FilePath == path)
                {
                    node = element;
                    break;
                }
                if (Directory.Exists(element.FilePath))
                {
                    node = FindTreeNodeElement(element, Path.GetDirectoryName(element.FilePath));
                }
            }

            return node;
        }

        private ExplorerElement FindTreeNodeDirectory(ExplorerElement parent, string directory)
        {
            ExplorerElement node = null;
            
            for (int i = 0; i < parent.Items.Count; i++)
            {
                ExplorerElement element = parent.Items[i] as ExplorerElement;
                if (Directory.Exists(element.FilePath))
                {
                    if(element.FilePath == directory)
                    {
                        node = element;
                        break;
                    } else
                    {
                        node = FindTreeNodeDirectory(element, Path.GetDirectoryName(element.FilePath));
                    }
                }
            }

            return node;
        }
    }
}
