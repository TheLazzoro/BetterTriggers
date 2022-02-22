using Facades.Containers;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Facades.Controllers
{
    public class ControllerProject
    {
        public void CreateProject(string language, string name, string destinationFolder)
        {
            War3Project project = new War3Project()
            {
                Name = name,
                Language = language,
                Header = "",
                Root = destinationFolder + @"\" + name,
                Files = new List<string>(),
            };

            string json = JsonConvert.SerializeObject(project);

            string filepath = destinationFolder + @"\" + name + ".json";
            File.WriteAllText(filepath, json);
            Directory.CreateDirectory(destinationFolder + @"\" + name);

            ContainerProject container = new ContainerProject();
            container.NewProject(project, filepath);
        }

        /// <summary>
        /// Loads all files from a given project into the container
        /// </summary>
        public War3Project LoadProject(string filepath)
        {
            ControllerRecentFiles controllerRecentFiles = new ControllerRecentFiles();

            if (!File.Exists(filepath))
            {
                controllerRecentFiles.RemoveRecentByPath(filepath);
                return null;
            }

            string json = File.ReadAllText(filepath);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(json);

            ContainerProject container = new ContainerProject();
            container.NewProject(project, filepath);
            // Loads all elements into the backend
            //ContainerFolders.AddFolder(new ExplorerElementFolder(project.Root));

            string[] filesInRoot = Directory.GetFileSystemEntries(project.Root);
            for (int i = 0; i < filesInRoot.Length; i++)
            {
                OnCreateElement(filesInRoot[i]);
            }

            controllerRecentFiles.AddProjectToRecent(filepath);

            return project;
        }

        public War3Project GetCurrentProject()
        {
            return ContainerProject.project;
        }

        public void OnCreateElement(string fullPath)
        {
            string directory = Path.GetDirectoryName(fullPath);

            ExplorerElementRoot root = ContainerProject.projectFiles[0] as ExplorerElementRoot;
            IExplorerElement parent = FindExplorerElementFolder(root, directory);

            RecurseCreateElement(parent, fullPath);
        }

        private void RecurseCreateElement(IExplorerElement parent, string fullPath)
        {
            IExplorerElement explorerElement = null;

            // Add item to appropriate container
            switch (Path.GetExtension(fullPath))
            {
                case "":
                    explorerElement = new ExplorerElementFolder(fullPath);
                    ContainerFolders.AddFolder(explorerElement as ExplorerElementFolder);
                    break;
                case ".trg":
                    explorerElement = new ExplorerElementTrigger(fullPath);
                    ContainerTriggers.AddTrigger(explorerElement as ExplorerElementTrigger);
                    break;
                case ".j":
                    explorerElement = new ExplorerElementScript(fullPath);
                    ContainerScripts.AddScript(explorerElement as ExplorerElementScript);
                    break;
                case ".var":
                    explorerElement = new ExplorerElementVariable(fullPath);
                    ContainerVariables.AddVariable(explorerElement as ExplorerElementVariable);
                    break;
                default:
                    break;
            }

            if (parent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)parent;
                root.explorerElements.Add(explorerElement);
            } else if(parent is ExplorerElementFolder)
            {
                var folder = (ExplorerElementFolder)parent;
                folder.explorerElements.Add(explorerElement);
            }

            // Recurse into the element if it's a folder
            if (Directory.Exists(fullPath))
            {
                string[] entries = Directory.GetFileSystemEntries(fullPath);
                for (int i = 0; i < entries.Length; i++)
                {
                    RecurseCreateElement((ExplorerElementFolder)explorerElement, entries[i]);
                }
            }
        }

        public void OnRenameElement(string oldFullPath, string newFullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);

            RecurseRenameElement(elementToRename, oldFullPath, newFullPath);

        }

        /// <summary>
        /// Needed when a folder containing files is renamed.
        /// </summary>
        /// <param name="elementToRename"></param>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        private void RecurseRenameElement(IExplorerElement elementToRename, string oldFullPath, string newFullPath)
        {
            if (elementToRename != null)
            {
                if (Directory.Exists(newFullPath))
                {
                    var folder = elementToRename as ExplorerElementFolder;

                    string[] entries = Directory.GetFileSystemEntries(newFullPath);
                    for (int i = 0; i < entries.Length; i++)
                    {
                        IExplorerElement elementInSubSearch = null;
                        string filename = Path.GetFileName(entries[i]);
                        
                        // find element with matching old path
                        for(int e = 0; e < folder.explorerElements.Count; e++)
                        {
                            if(filename == Path.GetFileName(folder.explorerElements[e].GetPath()))
                            {
                                elementInSubSearch = folder.explorerElements[e];
                                break;
                            }
                        }

                        // we need to loop through all items with the old path
                        RecurseRenameElement(elementInSubSearch, elementToRename.GetPath(), entries[i]);
                    }
                }

                elementToRename.SetPath(newFullPath);
                elementToRename.Notify();
            }
        }

        public void OnDeleteElement(string fullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToDelete = FindExplorerElement(rootNode, fullPath);

            RecurseDeleteElement(elementToDelete);
        }

        private void RecurseDeleteElement(IExplorerElement elementToDelete)
        {
            if (elementToDelete != null)
            {
                if (elementToDelete is ExplorerElementFolder)
                {
                    var folder = elementToDelete as ExplorerElementFolder;
                    // Delete all child elements (items in folders and all their subfoldes with item etc.)
                    for (int i = 0; i < folder.explorerElements.Count; i++)
                    {
                        RecurseDeleteElement(folder.explorerElements[i]);
                    }
                }

                // Remove item from container
                // Hack.
                ContainerFolders.RemoveByFilePath(elementToDelete.GetPath());
                ContainerTriggers.RemoveByFilePath(elementToDelete.GetPath());
                ContainerScripts.RemoveByFilePath(elementToDelete.GetPath());
                ContainerVariables.RemoveByFilePath(elementToDelete.GetPath());

                // Remove item from TriggerExplorer
                //var parent = (TreeItemExplorerElement)elementToDelete.Parent;
                //parent.Items.Remove(elementToDelete);

                // Remove tab item

                /*
                if (elementToDelete.tabItem != null)
                {
                    var tabParent = (TabControl)elementToDelete.tabItem.Parent;
                    tabParent.Items.Remove(elementToDelete.tabItem);
                }
                */

                elementToDelete.DeleteObservers();

                //notify()?
            }
        }

        public void OnElementChanged(string fullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToChange = FindExplorerElement(rootNode, fullPath);
            elementToChange.Notify();
        }

        public IExplorerElement FindExplorerElement(IExplorerElement parent, string path)
        {
            IExplorerElement matching = null;
            List<IExplorerElement> children = null;
            if (parent is ExplorerElementRoot)
            {
                var root = parent as ExplorerElementRoot; // defaults to root
                children = root.explorerElements;
            }
            else if (parent is ExplorerElementFolder)
            {
                var folder = parent as ExplorerElementFolder;
                children = folder.explorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                IExplorerElement element = children[i];
                if (element.GetPath() == path)
                {
                    matching = element;
                    break;
                }
                if (Directory.Exists(element.GetPath()) && matching == null)
                {
                    matching = FindExplorerElement((ExplorerElementFolder)element, path);
                }
            }

            return matching;
        }

        public IExplorerElement FindExplorerElementFolder(IExplorerElement parent, string directory)
        {
            IExplorerElement matching = parent;
            List<IExplorerElement> children = null;

            if (parent is ExplorerElementRoot)
            {
                var root = parent as ExplorerElementRoot; // defaults to root
                children = root.explorerElements;
            }
            else if (parent is ExplorerElementFolder)
            {
                var folder = parent as ExplorerElementFolder;
                children = folder.explorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                IExplorerElement element = children[i];
                if (Directory.Exists(element.GetPath()) && matching == null)
                {
                    if (element.GetPath() == directory)
                    {
                        matching = (ExplorerElementFolder) element;
                        break;
                    }
                    else
                    {
                        matching = FindExplorerElementFolder((ExplorerElementFolder)element, directory);
                    }
                }
            }

            return matching;
        }

        /*
        private void LoadFiles(string folder, TreeViewItem parentNode)
        {
            ControllerTrigger controllerTrigger = new ControllerTrigger();

            string[] entries = Directory.GetFileSystemEntries(folder);
            foreach (var entry in entries)
            {

                if (Directory.Exists(entry))
                {
                    ExplorerElement item = new ExplorerElement(entry);
                    ContainerFolders.AddTriggerElement(item);
                    parentNode.Items.Add(item);
                    TreeViewManipulator.SetTreeViewItemAppearance(item, Reader.GetFileNameAndExtension(entry), Category.Folder);
                    LoadFiles(entry, item);
                }
                else if (File.Exists(entry))
                {
                    ExplorerElement item = new ExplorerElement(entry);
                    parentNode.Items.Add(item);

                    switch (Reader.GetFileExtension(entry))
                    {
                        case ".trg":
                            ContainerTriggers.AddTriggerElement(item);
                            break;
                        case ".j":
                            ContainerScripts.AddTriggerElement(item);
                            break;
                        case ".var":
                            ContainerVariables.AddTriggerElement(item);
                            break;
                        default:
                            break;
                    }
                    //var file = File.ReadAllText(entry);
                    //Model.Trigger trigger = JsonConvert.DeserializeObject<Model.Trigger>(file);

                    //controllerTrigger.CreateTriggerWithElements(triggerExplorer, mainGrid, entry, trigger);
                }
            }
        }

        public void OnClick_ExplorerElement(ExplorerElement selectedElement, TabControl tabControl)
        {
            if (selectedElement != null && selectedElement.Ielement == null) // Load file data if the element is null
            {
                switch (Reader.GetFileExtension(selectedElement.FilePath))
                {
                    case ".trg":
                        ControllerTrigger triggerController = new ControllerTrigger();
                        Model.Trigger trigger = triggerController.LoadTriggerFromFile(selectedElement.FilePath);
                        var triggerControl = triggerController.CreateTriggerWithElements(tabControl, trigger);
                        TabItemBT tabItemTrigger = new TabItemBT(triggerControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemTrigger);
                        //tabControl.ItemsSource.
                        selectedElement.tabItem = tabItemTrigger;
                        selectedElement.Ielement = triggerControl;
                        break;
                    case ".var":
                        ControllerVariable controllerVariable = new ControllerVariable();
                        Model.Data.Variable variable = controllerVariable.LoadVariableFromFile(selectedElement.FilePath);
                        variable.Name = Path.GetFileNameWithoutExtension(selectedElement.FilePath); // hack
                        var variableControl = controllerVariable.CreateVariableWithElements(tabControl, variable);
                        TabItemBT tabItemVariable = new TabItemBT(variableControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemVariable);
                        //tabControl.ItemsSource.
                        selectedElement.tabItem = tabItemVariable;
                        selectedElement.Ielement = variableControl;
                        break;
                    case ".j":
                        ControllerScript scriptController = new ControllerScript();
                        var scripControl = scriptController.CreateScriptControlWithScript(tabControl, selectedElement.FilePath);
                        TabItemBT tabItemScript = new TabItemBT(scripControl, Reader.GetFileName(selectedElement.FilePath));
                        tabControl.Items.Add(tabItemScript);
                        selectedElement.tabItem = tabItemScript;
                        selectedElement.Ielement = scripControl;
                        break;
                    default:
                        break;
                }
            }
            if (selectedElement != null && selectedElement.Ielement != null)
            {
                currentExplorerElement = selectedElement;
                selectedElement.Ielement.OnElementClick();
                tabControl.SelectedItem = selectedElement.tabItem;
            }
        }

        public string GetDirectoryFromSelection(TreeView treeViewTriggerExplorer)
        {
            ExplorerElement selectedItem = treeViewTriggerExplorer.SelectedItem as ExplorerElement;

            if (selectedItem == null)
                return null;

            if (Directory.Exists(selectedItem.FilePath))
                return selectedItem.FilePath;

            else if (File.Exists(selectedItem.FilePath))
                return Path.GetDirectoryName(selectedItem.FilePath);

            return null;
        }
        */
    }
}
