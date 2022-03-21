using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Commands;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using GUI.Container;
using Model.EditorData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTriggerExplorer
    {
        public void Populate(TriggerExplorer te)
        {
            var root = ContainerProject.projectFiles[0] as ExplorerElementRoot;



            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                RecursePopulate(te, te.map, root.explorerElements[i]);
            }
        }

        private void RecursePopulate(TriggerExplorer te, TreeItemExplorerElement parent, IExplorerElement element)
        {
            var treeItem = new TreeItemExplorerElement(element);
            element.Attach(treeItem); // attach treeItem to element so it can respond to events happening to the element.
            parent.Items.Add(treeItem);

            if (element is ExplorerElementFolder)
            {
                var folder = element as ExplorerElementFolder;
                for (int i = 0; i < folder.explorerElements.Count; i++)
                {
                    var child = folder.explorerElements[i];
                    RecursePopulate(te, treeItem, child);
                }
            }
        }

        public void SaveAll()
        {
            var unsaved = ContainerUnsavedElements.UnsavedElements;
            for(int i = 0; i < unsaved.Count; i++)
            {
                var element = unsaved[i];
                element.Save();
            }

            unsaved.Clear();

            ControllerProject controller = new ControllerProject();
            controller.SaveProject();
        }

        public void OnSelectItem(TreeItemExplorerElement selectedItem, DragableTabControl dragableTabControl)
        {
            if (selectedItem.editor == null)
            {
                switch (Path.GetExtension(selectedItem.Ielement.GetPath())) // hack
                {
                    case "":
                        try
                        {
                            var categoryControl = new CategoryControl((ExplorerElementFolder)selectedItem.Ielement);
                            categoryControl.Attach(selectedItem);
                            selectedItem.editor = categoryControl;
                        }
                        catch (Exception e)
                        {
                            var rootControl = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
                            rootControl.Attach(selectedItem);
                            selectedItem.editor = rootControl;
                        }
                        break;
                    case ".trg":
                        ControllerTrigger controllerTrigger = new ControllerTrigger();
                        var triggerControl = new TriggerControl((ExplorerElementTrigger)selectedItem.Ielement);
                        triggerControl.Attach(selectedItem);
                        selectedItem.editor = triggerControl;
                        break;
                    case ".j":
                        ControllerScript controllerScript = new ControllerScript();
                        var scriptControl = new ScriptControl((ExplorerElementScript)selectedItem.Ielement);
                        scriptControl.Attach(selectedItem);
                        selectedItem.editor = scriptControl;
                        break;
                    case ".var":
                        ControllerVariable controllerVariable = new ControllerVariable();
                        var variableControl = new VariableControl(controllerVariable.GetExplorerElementVariableInMemory(selectedItem.Ielement.GetPath()), selectedItem.Ielement.GetName());
                        variableControl.Attach(selectedItem);
                        selectedItem.editor = variableControl;
                        break;
                    case ".json":
                        var rootControl2 = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
                        rootControl2.Attach(selectedItem);
                        selectedItem.editor = rootControl2;
                        break;
                    default:
                        break;
                }

                TabItemBT tabItem = new TabItemBT(selectedItem.editor, selectedItem.Ielement.GetName());
                selectedItem.tabItem = tabItem;
                dragableTabControl.tabControl.Items.Add(tabItem);
            }

            if (selectedItem.editor != null)
                selectedItem.editor.Refresh();
            if (selectedItem.tabItem != null)
                dragableTabControl.tabControl.SelectedItem = selectedItem.tabItem;
        }


        public void OnCreateElement(TriggerExplorer te, string fullPath)
        {
            CommandExplorerElementCreate command = new CommandExplorerElementCreate(te, fullPath);
            command.Execute();
        }

        public void RecurseCreateElement(IExplorerElement folder, TreeItemExplorerElement parent, string fullPath, bool doRecurse = true, bool doInsert = false, int insertIndex = 0)
        {
            ControllerProject controller = new ControllerProject();
            IExplorerElement createdElement = controller.FindExplorerElement(folder, fullPath);

            // Create ExplorerElement in the parent node
            TreeItemExplorerElement treeElement = new TreeItemExplorerElement(createdElement);
            createdElement.Attach(treeElement);
            if(doInsert)
                parent.Items.Insert(insertIndex, treeElement);
            else
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

            if (!doRecurse)
                return;

            // Recurse into the element if it's a folder
            if (Directory.Exists(fullPath))
            {
                string[] entries = Directory.GetFileSystemEntries(fullPath);
                for (int i = 0; i < entries.Length; i++)
                {
                    RecurseCreateElement((ExplorerElementFolder)createdElement, treeElement, entries[i]);
                }
            }
        }

        internal void OnMoveElement(TriggerExplorer te, string fullPath, int insertIndex)
        {
            TreeItemExplorerElement elementToMove = FindTreeNodeElement(te.map, fullPath);
            TreeItemExplorerElement oldParent = elementToMove.Parent as TreeItemExplorerElement;
            TreeItemExplorerElement newParent = FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));


            oldParent.Items.Remove(elementToMove);
            newParent.Items.Insert(insertIndex, elementToMove);
            elementToMove.IsSelected = true;
        }

        internal void OnDeleteElement(TriggerExplorer te, string fullPath)
        {
            CommandExplorerElementDelete command = new CommandExplorerElementDelete(te, fullPath);
            command.Execute();
        }

        internal TreeItemExplorerElement FindTreeNodeElement(TreeItemExplorerElement parent, string path)
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

        internal TreeItemExplorerElement FindTreeNodeDirectory(TreeItemExplorerElement parent, string directory)
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
