using BetterTriggers.Containers;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterTriggers.Controllers
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
                Files = new List<War3ProjectFileEntry>()
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
            var projectRootEntry = new War3ProjectFileEntry()
            {
                isEnabled = true,
                isInitiallyOn = true,
                path = project.Root,
                Files = project.Files,
            };

            // get all files
            string[] files = Directory.GetFileSystemEntries(project.Root, "*", SearchOption.AllDirectories);
            List<string> fileCheckList = new List<string>();
            fileCheckList.AddRange(files);

            // Recurse through elements found in the project file
            RecurseLoad(projectRootEntry, ContainerProject.projectFiles[0], fileCheckList);

            // Loop through elements not found
            for(int i = 0; i < fileCheckList.Count; i++)
            {
                OnCreateElement(fileCheckList[i], false);
            }

            controllerRecentFiles.AddProjectToRecent(filepath);

            return project;
        }

        private void RecurseLoad(War3ProjectFileEntry entryParent, IExplorerElement elementParent, List<string> fileCheckList)
        {
            List<IExplorerElement> elementChildren = null;
            if (elementParent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)elementParent;
                elementChildren = root.explorerElements;
            }
            else if (elementParent is ExplorerElementFolder)
            {
                var folder = (ExplorerElementFolder)elementParent;
                elementChildren = folder.explorerElements;
            }

            for (int i = 0; i < entryParent.Files.Count; i++)
            {
                var entryChild = entryParent.Files[i];
                IExplorerElement explorerElementChild = null;

                if (File.Exists(entryChild.path) || Directory.Exists(entryChild.path))
                {
                    fileCheckList.Remove(entryChild.path);

                    // Add item to appropriate container
                    switch (Path.GetExtension(entryChild.path))
                    {
                        case "":
                            explorerElementChild = new ExplorerElementFolder(entryChild.path);
                            ContainerFolders.AddFolder(explorerElementChild as ExplorerElementFolder);
                            break;
                        case ".trg":
                            explorerElementChild = new ExplorerElementTrigger(entryChild.path);
                            ContainerTriggers.AddTrigger(explorerElementChild as ExplorerElementTrigger);
                            break;
                        case ".j":
                            explorerElementChild = new ExplorerElementScript(entryChild.path);
                            ContainerScripts.AddScript(explorerElementChild as ExplorerElementScript);
                            break;
                        case ".var":
                            explorerElementChild = new ExplorerElementVariable(entryChild.path);
                            ContainerVariables.AddVariable(explorerElementChild as ExplorerElementVariable);
                            break;
                        default:
                            break;
                    }

                    elementChildren.Add(explorerElementChild);
                    if (Directory.Exists(explorerElementChild.GetPath()))
                        RecurseLoad(entryChild, explorerElementChild, fileCheckList);
                }
            }

        }

        public void SaveProject()
        {
            var root = (ExplorerElementRoot)ContainerProject.projectFiles[0];
            var project = ContainerProject.project;
            project.Files = new List<War3ProjectFileEntry>();


            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                var element = root.explorerElements[i];

                var fileEntry = new War3ProjectFileEntry();
                fileEntry.path = element.GetPath();
                fileEntry.isEnabled = element.GetEnabled();
                fileEntry.isInitiallyOn = element.GetInitiallyOn();

                project.Files.Add(fileEntry);

                if (element is ExplorerElementFolder)
                {
                    RecurseSaveFileEntries((ExplorerElementFolder)element, fileEntry);
                }
            }

            var json = JsonConvert.SerializeObject(project);
            File.WriteAllText(root.path, json);
        }

        private void RecurseSaveFileEntries(ExplorerElementFolder parent, War3ProjectFileEntry parentEntry)
        {
            List<IExplorerElement> children = parent.explorerElements;

            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];

                // TODO: DUPLICATE CODE!!
                var fileEntryChild = new War3ProjectFileEntry();
                fileEntryChild.path = element.GetPath();
                fileEntryChild.isEnabled = element.GetEnabled();
                fileEntryChild.isInitiallyOn = element.GetInitiallyOn();

                parentEntry.Files.Add(fileEntryChild);

                if (element is ExplorerElementFolder)
                {
                    RecurseSaveFileEntries((ExplorerElementFolder)element, fileEntryChild);
                }
            }
        }

        public War3Project GetCurrentProject()
        {
            return ContainerProject.project;
        }

        public void SetElementEnabled(IExplorerElement element, bool isEnabled)
        {
            element.SetEnabled(isEnabled);
        }

        public void SetElementInitiallyOn(IExplorerElement element, bool isInitiallyOn)
        {
            element.SetInitiallyOn(isInitiallyOn);
        }

        public void OnCreateElement(string fullPath, bool doRecurse = true)
        {
            string directory = Path.GetDirectoryName(fullPath);

            ExplorerElementRoot root = ContainerProject.projectFiles[0] as ExplorerElementRoot;
            IExplorerElement parent = FindExplorerElementFolder(root, directory);

            RecurseCreateElement(parent, fullPath, doRecurse);
        }

        private void RecurseCreateElement(IExplorerElement parent, string fullPath, bool doRecurse)
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
            }
            else if (parent is ExplorerElementFolder)
            {
                var folder = (ExplorerElementFolder)parent;
                folder.explorerElements.Add(explorerElement);
            }

            if (!doRecurse)
                return;

            // Recurse into the element if it's a folder
            if (Directory.Exists(fullPath))
            {
                string[] entries = Directory.GetFileSystemEntries(fullPath);
                for (int i = 0; i < entries.Length; i++)
                {
                    RecurseCreateElement((ExplorerElementFolder)explorerElement, entries[i], doRecurse);
                }
            }
        }

        /// <summary>
        /// This is also used when files are moved, hence why we need
        /// to detach it from its current parent and attach it to the other.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        public void OnRenameElement(string oldFullPath, string newFullPath, int insertIndex)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);
            IExplorerElement oldParent = FindExplorerElementFolder(rootNode, Path.GetDirectoryName(oldFullPath));
            IExplorerElement newParent = FindExplorerElementFolder(rootNode, Path.GetDirectoryName(newFullPath));

            oldParent.RemoveFromList(elementToRename);
            newParent.InsertIntoList(elementToRename, insertIndex);

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
                        for (int e = 0; e < folder.explorerElements.Count; e++)
                        {
                            if (filename == Path.GetFileName(folder.explorerElements[e].GetPath()))
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

                elementToDelete.DeleteObservers();

                //notify()?
            }
        }

        /// <summary>
        /// Used when an element is rearranged within it's current folder.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="index"></param>
        public void RearrangeElement(IExplorerElement element, int insertIndex)
        {
            var parent = FindExplorerElementFolder(ContainerProject.projectFiles[0], Path.GetDirectoryName(element.GetPath()));

            if (parent is ExplorerElementRoot)
            {
                var root = (ExplorerElementRoot)parent;
                root.RemoveFromList(element);
                root.InsertIntoList(element, insertIndex);
            }
            else if (parent is ExplorerElementFolder)
            {
                var folder = (ExplorerElementFolder)parent;
                folder.RemoveFromList(element);
                folder.InsertIntoList(element, insertIndex);
            }
        }

        public void OnElementChanged(string fullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToChange = FindExplorerElement(rootNode, fullPath);

            if (elementToChange != null)
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
            IExplorerElement matching = null;
            List<IExplorerElement> children = null;

            if (parent is ExplorerElementRoot)
            {
                var root = parent as ExplorerElementRoot;
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
                if (Directory.Exists(element.GetPath()) && (matching == null || matching is ExplorerElementRoot))
                {
                    if (element.GetPath() == directory)
                    {
                        matching = (ExplorerElementFolder)element;
                        break;
                    }
                    else
                    {
                        matching = FindExplorerElementFolder((ExplorerElementFolder)element, directory);
                    }
                }
            }

            // Returns root if no matching parent node was found.
            // Usually only need when loading a new project.
            if (matching == null)
                return ContainerProject.projectFiles[0];

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
