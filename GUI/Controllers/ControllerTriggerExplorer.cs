﻿using Facades.Containers;
using Facades.Controllers;
using GUI.Components;
using GUI.Components.TriggerExplorer;
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

        public void OnSelectItem(TreeItemExplorerElement selectedItem, DragableTabControl dragableTabControl)
        {
            if (selectedItem.editor == null)
            {
                switch (Path.GetExtension(selectedItem.Ielement.GetPath())) // hack
                {
                    case "":
                        try
                        {
                            selectedItem.editor = new CategoryControl((ExplorerElementFolder)selectedItem.Ielement);
                        }
                        catch (Exception e)
                        {
                            selectedItem.editor = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
                        }
                        break;
                    case ".trg":
                        ControllerTrigger controllerTrigger = new ControllerTrigger();
                        selectedItem.editor = new TriggerControl((ExplorerElementTrigger)selectedItem.Ielement);
                        break;
                    case ".j":
                        ControllerScript controllerScript = new ControllerScript();
                        selectedItem.editor = new ScriptControl((ExplorerElementScript)selectedItem.Ielement);
                        break;
                    case ".var":
                        ControllerVariable controllerVariable = new ControllerVariable();
                        selectedItem.editor = new VariableControl(controllerVariable.GetExplorerElementVariableInMemory(selectedItem.Ielement.GetPath()), selectedItem.Ielement.GetName());
                        break;
                    case ".json":
                        selectedItem.editor = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
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
            ControllerProject controller = new ControllerProject();
            IExplorerElement folder = controller.FindExplorerElementFolder(ContainerProject.projectFiles[0], fullPath);
            TreeItemExplorerElement parent = FindTreeNodeDirectory(te.map, Path.GetDirectoryName(fullPath));
            if (parent == null)
                parent = te.map;

            RecurseCreateElement(folder, parent, fullPath);
        }

        private void RecurseCreateElement(IExplorerElement folder, TreeItemExplorerElement parent, string fullPath)
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