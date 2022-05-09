using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Components;
using GUI.Components.TriggerExplorer;
using GUI.Container;
using Model.EditorData;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerTriggerExplorer
    {
        public void Populate(TriggerExplorer te)
        {
            ControllerProject controllerProject = new ControllerProject();
            var root = controllerProject.GetProjectRoot() as ExplorerElementRoot;
            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                RecursePopulate(te, te.map, root.explorerElements[i]);
            }
        }

        public void RecursePopulate(TriggerExplorer te, TreeItemExplorerElement parent, IExplorerElement element)
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

        internal TriggerExplorer GetCurrentExplorer()
        {
            return TriggerExplorer.Current;
        }

        public void SaveAll()
        {
            var unsaved = ContainerUnsavedElements.UnsavedElements;
            for (int i = 0; i < unsaved.Count; i++)
            {
                var element = unsaved[i];
                element.Save();
            }

            unsaved.Clear();

            ControllerProject controller = new ControllerProject();
            controller.SaveProject();
        }

        public void OnSelectItem(TreeItemExplorerElement selectedItem, TabControl_BT tabControl)
        {
            if (selectedItem.editor == null || selectedItem.tabItem == null)
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
                tabControl.tabControl.Items.Add(tabItem);
            }

            if (selectedItem.editor != null)
                selectedItem.editor.Refresh();
            if (selectedItem.tabItem != null)
                tabControl.tabControl.SelectedItem = selectedItem.tabItem;
        }


        public void OnCreateElement(TriggerExplorer te, string fullPath)
        {
            ControllerProject controllerProject = new ControllerProject();
            var explorerElement = controllerProject.FindExplorerElement(controllerProject.GetProjectRoot(), fullPath);
            int insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);



            TreeItemExplorerElement treeItemExplorerElement = new TreeItemExplorerElement(explorerElement);
            explorerElement.Attach(treeItemExplorerElement);
            treeItemExplorerElement.OnCreated(insertIndex);
        }

        internal void OnMoveElement(TriggerExplorer te, string fullPath, int insertIndex)
        {
            TreeItemExplorerElement elementToMove = FindTreeNodeElement(te.map, fullPath);
            TreeItemExplorerElement oldParent = elementToMove.Parent as TreeItemExplorerElement;
            TreeItemExplorerElement newParent = FindTreeNodeDirectory(Path.GetDirectoryName(fullPath));
            
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (newParent == null) // hack. idk why it fires twice
                    return;

                oldParent.Items.Remove(elementToMove);
                newParent.Items.Insert(insertIndex, elementToMove);
                elementToMove.IsSelected = true;
            });
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

        internal TreeItemExplorerElement FindTreeNodeDirectory(string directory)
        {
            if (directory == GetCurrentExplorer().map.Ielement.GetPath())
                return GetCurrentExplorer().map;

            return FindTreeNodeDirectory(GetCurrentExplorer().map, directory);
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
