using Facades.Containers;
using Facades.Controllers;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using Model.EditorData;
using System.IO;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTriggerExplorer
    {
        public void Populate(TriggerExplorer te)
        {
            RecursePopulate(te.map, ContainerFolders.Get(0));
        }

        private void RecursePopulate(TreeItemExplorerElement parent, ExplorerElementFolder folder)
        {
            for (int i = 0; i < folder.explorerElements.Count; i++)
            {
                var element = folder.explorerElements[i];
                var treeItem = new TreeItemExplorerElement(element);
                parent.Items.Add(treeItem);
                element.Attach(treeItem); // attach treeItem to element so it can respond to events happening to the element.

                if (element is ExplorerElementFolder)
                {
                    RecursePopulate(treeItem, element as ExplorerElementFolder);
                }
            }
        }

        public void OnSelectItem(TreeItemExplorerElement selectedItem, TabControl tabControl)
        {
            if (selectedItem.editor == null)
            {
                switch (Path.GetExtension(selectedItem.Ielement.GetPath()))
                {
                    case "":
                        selectedItem.editor = null; // TODO
                        break;
                    case ".trg":
                        ControllerTrigger controllerTrigger = new ControllerTrigger();
                        selectedItem.editor = new TriggerControl(controllerTrigger.LoadTriggerFromFile(selectedItem.Ielement.GetPath()));
                        break;
                    case ".j":
                        ControllerScript controllerScript = new ControllerScript();
                        selectedItem.editor = new ScriptControl(controllerScript.LoadScriptFromFile(selectedItem.Ielement.GetPath()));
                        break;
                    case ".var":
                        ControllerVariable controllerVariable = new ControllerVariable();
                        selectedItem.editor = new VariableControl(controllerVariable.GetVariableInMemory(selectedItem.Ielement.GetPath()));
                        break;
                    default:
                        break;
                }

                TabItemBT tabItem = new TabItemBT(selectedItem.editor, selectedItem.Ielement.GetName());
                selectedItem.tabItem = tabItem;
                tabControl.Items.Add(tabItem);
            }

            if(selectedItem.tabItem != null)
                tabControl.SelectedItem = selectedItem.tabItem;
        }


        
        public void OnCreateElement(TriggerExplorer te, string fullPath)
        {
            ControllerProject controller = new ControllerProject();
            ExplorerElementFolder folder = controller.FindExplorerElementFolder(ContainerFolders.Get(0), fullPath);
            TreeItemExplorerElement parent = FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));

            RecurseCreateElement(folder, parent, fullPath);
        }

        private void RecurseCreateElement(ExplorerElementFolder folder, TreeItemExplorerElement parent, string fullPath)
        {
            ControllerProject controller = new ControllerProject();
            IExplorerElement createdElement = controller.FindExplorerElement(folder, fullPath);

            // Create ExplorerElement in the parent node
            TreeItemExplorerElement treeElement = new TreeItemExplorerElement(createdElement);
            createdElement.Attach(treeElement);
            parent.Items.Insert(parent.Items.Count, treeElement);

            // Add item to appropriate container
            switch (Path.GetExtension(createdElement.GetPath()))
            {
                case "":
                    ContainerFolders.AddFolder(createdElement as ExplorerElementFolder);
                    break;
                case ".trg":
                    ContainerTriggers.AddTrigger(createdElement as ExplorerElementTrigger);
                    break;
                case ".j":
                    ContainerScripts.AddScript(createdElement as ExplorerElementScript);
                    break;
                case ".var":
                    ContainerVariables.AddVariable(createdElement as ExplorerElementVariable);
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
                    RecurseCreateElement(createdElement as ExplorerElementFolder, treeElement, entries[i]);
                }
            }
        }

        private TreeItemExplorerElement FindTreeNodeElement(TreeItemExplorerElement parent, string path)
        {
            TreeItemExplorerElement node = null;

            for (int i = 0; i < parent.Items.Count; i++)
            {
                TreeItemExplorerElement element = parent.Items[i] as TreeItemExplorerElement;
                if (element.Ielement.GetPath() == path)
                {
                    node = element;
                    break;
                }
                if (Directory.Exists(element.Ielement.GetPath()) && node == null)
                {
                    node = FindTreeNodeElement(element, path);
                }
            }

            return node;
        }

        private TreeItemExplorerElement FindTreeNodeDirectory(TreeItemExplorerElement parent, string directory)
        {
            TreeItemExplorerElement node = null;

            for (int i = 0; i < parent.Items.Count; i++)
            {
                TreeItemExplorerElement element = parent.Items[i] as TreeItemExplorerElement;
                if (Directory.Exists(element.Ielement.GetPath()) && node == null)
                {
                    if (element.Ielement.GetPath() == directory)
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
