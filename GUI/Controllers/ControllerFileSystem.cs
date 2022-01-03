using GUI.Components.TriggerExplorer;
using GUI.Containers;
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

        // TODO:
        // This is currently only implemented for .json "trigger" files
        public void CreateElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);

            // Find matching directory
            var rootNode = triggerExplorer.map;
            ExplorerElement parent = FindTreeNodeDirectory(rootNode, directory);
            if (parent == null)
                parent = rootNode;

            // Create ExplorerElement in the parent node
            ExplorerElement explorerElement = new ExplorerElement(fullPath);
            parent.Items.Insert(parent.Items.Count, explorerElement);

            // Add item to appropriate container
            switch (Path.GetExtension(explorerElement.FilePath))
            {
                case "":
                    ContainerFolders.AddTriggerElement(explorerElement);
                    break;
                case ".json":
                    ContainerTriggers.AddTriggerElement(explorerElement);
                    break;
                case ".j":
                    ContainerScripts.AddTriggerElement(explorerElement);
                    break;
                default:
                    break;
            }
        }

        public void DeleteElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            var rootNode = triggerExplorer.map;
            ExplorerElement elementToDelete = FindTreeNodeElement(rootNode, fullPath);

            if (elementToDelete != null)
            {
                var parent = (ExplorerElement)elementToDelete.Parent;
                parent.Items.Remove(elementToDelete);

                // Remove tab item
                if (elementToDelete.tabItem != null)
                {
                    var tabParent = (TabControl)elementToDelete.tabItem.Parent;
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
                if (element.FilePath == path)
                {
                    node = element;
                    break;
                }
                if (Directory.Exists(element.FilePath))
                {
                    node = FindTreeNodeElement(element, path);
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
                    if (element.FilePath == directory)
                    {
                        node = element;
                        break;
                    }
                    else
                    {
                        node = FindTreeNodeDirectory(element, directory);
                    }
                }
            }

            return node;
        }
    }
}
