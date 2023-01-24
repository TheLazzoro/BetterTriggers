using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using GUI.Components;
using GUI.Components.TriggerExplorer;
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
                if (folder.isExpanded)
                    treeItem.ExpandSubtree();

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
            ControllerProject controller = new ControllerProject();
            controller.SaveProject();
        }

        public void OnSelectTab(TreeItemExplorerElement selectedItem, TabViewModel tabViewModel, TabControl tabControl)
        {
            if (selectedItem.Ielement is ExplorerElementTrigger exTrig)
                ControllerTrigger.SelectedTrigger = exTrig.trigger;

            if (selectedItem.editor == null || selectedItem.tabItem == null)
            {
                if(selectedItem.Ielement is ExplorerElementRoot)
                {
                    var rootControl = new RootControl((ExplorerElementRoot)selectedItem.Ielement);
                    rootControl.Attach(selectedItem);
                    selectedItem.editor = rootControl;
                }
                else if (selectedItem.Ielement is ExplorerElementTrigger)
                {
                    var triggerControl = new TriggerControl((ExplorerElementTrigger)selectedItem.Ielement);
                    triggerControl.Attach(selectedItem);
                    selectedItem.editor = triggerControl;
                }
                else if (selectedItem.Ielement is ExplorerElementScript)
                {
                    var scriptControl = new ScriptControl((ExplorerElementScript)selectedItem.Ielement);
                    scriptControl.Attach(selectedItem);
                    selectedItem.editor = scriptControl;
                }
                else if (selectedItem.Ielement is ExplorerElementVariable)
                {
                    var element = (ExplorerElementVariable)selectedItem.Ielement;
                    var variableControl = new VariableControl(element.variable);
                    variableControl.Attach(selectedItem);
                    selectedItem.editor = variableControl;
                }

                // select already open tab
                for (int i = 0; i < tabViewModel.Tabs.Count; i++)
                {
                    var tab = tabViewModel.Tabs[i];
                    if(tab.explorerElement.Ielement.GetPath() == selectedItem.Ielement.GetPath())
                    {
                        selectedItem.tabItem = tab;
                        tabViewModel.Tabs.IndexOf(selectedItem.tabItem);
                        return;
                    }
                }

                if (selectedItem.editor == null)
                    return;

                TabItemBT tabItem = new TabItemBT(selectedItem, tabViewModel);
                tabViewModel.Tabs.Add(tabItem);
                selectedItem.tabItem = tabItem;
            }

            if (selectedItem.tabItem != null)
            {
                if(!tabViewModel.Tabs.Contains(selectedItem.tabItem))
                    tabViewModel.Tabs.Add(selectedItem.tabItem);

                tabControl.SelectedIndex = tabViewModel.Tabs.IndexOf(selectedItem.tabItem);
            }
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
