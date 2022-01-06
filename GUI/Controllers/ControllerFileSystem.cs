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
        public void MoveFile(ExplorerElement elementToMove, ExplorerElement target)
        {
            string directory = target.FilePath;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(target.FilePath);

            if (File.Exists(elementToMove.FilePath))
                File.Move(elementToMove.FilePath, directory + "/" + Path.GetFileName(elementToMove.FilePath));
            else if (Directory.Exists(elementToMove.FilePath))
                Directory.Move(elementToMove.FilePath, directory + "/" + Path.GetFileName(elementToMove.FilePath));
        }

        // TODO:
        // This is currently only implemented for .json "trigger" files
        // When .json "variable" files are in place this needs a rework
        public void OnCreateElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);

            // Find matching directory
            var rootNode = triggerExplorer.map;
            ExplorerElement parent = FindTreeNodeDirectory(rootNode, directory);
            if (parent == null)
                parent = rootNode;

            RecurseCreateElement(parent, fullPath);
        }

        private void RecurseCreateElement(ExplorerElement parent, string fullPath)
        {
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

            // Recurse into the element if it's a folder
            if (Directory.Exists(fullPath))
            {
                string[] entries = Directory.GetFileSystemEntries(fullPath);
                for (int i = 0; i < entries.Length; i++)
                {
                    RecurseCreateElement(explorerElement, entries[i]);
                }
            }
        }

        public void OnRenameElement(TriggerExplorer triggerExplorer, string oldFullPath, string newFullPath)
        {
            var rootNode = triggerExplorer.map;
            ExplorerElement elementToRename = FindTreeNodeElement(rootNode, oldFullPath);

            RecurseRenameElement(elementToRename, oldFullPath, newFullPath);

        }

        /// <summary>
        /// Needed when a folder containing files is renamed.
        /// </summary>
        /// <param name="elementToRename"></param>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        private void RecurseRenameElement(ExplorerElement elementToRename, string oldFullPath, string newFullPath)
        {
            if (elementToRename != null)
            {
                if (Directory.Exists(newFullPath))
                {
                    string[] entries = Directory.GetFileSystemEntries(newFullPath);
                    for (int i = 0; i < entries.Length; i++)
                    {
                        var child = elementToRename.Items[i] as ExplorerElement;
                        RecurseRenameElement(child, elementToRename.FilePath, entries[i]);
                    }
                }

                elementToRename.FilePath = newFullPath;
                elementToRename.ElementName = Path.GetFileNameWithoutExtension(newFullPath);
                elementToRename.RefreshElement();
            }
        }

        public void OnMoveElement(TriggerExplorer triggerExplorer, string oldFullPath, string newFullPath)
        {
            var rootNode = triggerExplorer.map;
            ExplorerElement elementToMove = FindTreeNodeElement(rootNode, oldFullPath);

            string fileContent = elementToMove.Ielement.GetSaveString();

        }

        public void OnDeleteElement(TriggerExplorer triggerExplorer, string fullPath)
        {
            var rootNode = triggerExplorer.map;
            ExplorerElement elementToDelete = FindTreeNodeElement(rootNode, fullPath);

            RecurseDeleteElement(elementToDelete);
        }

        private void RecurseDeleteElement(ExplorerElement elementToDelete)
        {
            if (elementToDelete != null)
            {
                if (elementToDelete.Items.Count > 0)
                {
                    // Delete all child elements (items in folders and all their subfoldes with item etc.)
                    for (int i = 0; i < elementToDelete.Items.Count; i++)
                    {
                        RecurseDeleteElement(elementToDelete.Items[i] as ExplorerElement);
                    }
                }

                // Remove item from container
                // Hack.
                ContainerFolders.RemoveByFilePath(elementToDelete.FilePath);
                ContainerTriggers.RemoveByFilePath(elementToDelete.FilePath);
                ContainerScripts.RemoveByFilePath(elementToDelete.FilePath);
                ContainerVariables.RemoveByFilePath(elementToDelete.FilePath);

                // Remove item from TriggerExplorer
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
                if (Directory.Exists(element.FilePath) && node == null)
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
                if (Directory.Exists(element.FilePath) && node == null)
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
