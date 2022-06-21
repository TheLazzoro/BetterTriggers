using BetterTriggers.Commands;
using BetterTriggers.Containers;
using Model.EditorData;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using War3Net.Build;
using War3Net.IO.Mpq;

namespace BetterTriggers.Controllers
{
    public class ControllerProject
    {
        /// <summary>
        /// </summary>
        /// <returns>Path to the project file.</returns>
        public string CreateProject(string language, string name, string destinationFolder)
        {
            string root = Path.Combine(destinationFolder, name);
            string src = Path.Combine(root, "src");
            string map = Path.Combine(root, "map");
            string dist = Path.Combine(root, "dist");
            string projectPath = Path.Combine(root, name + ".json");

            War3Project project = new War3Project()
            {
                Name = name,
                Language = language,
                Header = "",
                Root = src,
                Files = new List<War3ProjectFileEntry>()
            };

            string projectFile = JsonConvert.SerializeObject(project);

            Directory.CreateDirectory(root);
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(map);
            Directory.CreateDirectory(dist);
            File.WriteAllText(projectPath, projectFile);

            ContainerProject container = new ContainerProject();
            container.NewProject(project, projectPath);

            return projectPath;
        }

        public int GetUnsavedFileCount()
        {
            return ContainerUnsavedFiles.Count();
        }

        public bool War3MapDirExists()
        {
            bool exists = false;
            string dir = ContainerProject.project.War3MapDirectory;
            if (dir != null && dir != "" && Directory.Exists(dir) && File.Exists(Path.Combine(dir, "war3map.w3i")))
                exists = true;

            return exists;
        }

        public void SetWar3MapDir(string mapDir)
        {
            ContainerProject.project.War3MapDirectory = mapDir;
        }

        /// <summary>
        /// Builds an MPQ archive.
        /// </summary>
        /// <returns>Full path of the archive.</returns>
        public string BuildMap(string destinationDir = null)
        {
            ControllerScriptGenerator scriptGenerator = new ControllerScriptGenerator();
            scriptGenerator.GenerateScript();

            string mapDir = ContainerProject.project.War3MapDirectory;
            var map = Map.Open(mapDir);
            MapBuilder builder = new MapBuilder(map);

            string rootDir = Path.GetDirectoryName(ContainerProject.project.Root);
            string fullPath = string.Empty;
            if (destinationDir == null)
                fullPath = Path.Combine(rootDir, Path.Combine("dist", Path.GetFileName(mapDir)));
            else
            {
                Settings settings = Settings.Load();
                fullPath = Path.Combine(destinationDir, settings.CopyLocation + ".w3x");
            }

            builder.Build(fullPath);

            bool didWrite = false;
            int exeptions = 0;
            while (!didWrite && exeptions < 10)
            {
                Thread.Sleep(10);

                try
                {
                    builder.Build(fullPath);
                    didWrite = true;

                } catch(Exception ex) { exeptions++; }
            }
            if (!didWrite)
                throw new Exception("Could not write to map.");

            return fullPath;
        }

        public void TestMap()
        {
            string destinationDir = Path.GetTempPath();
            string fullPath = BuildMap(destinationDir);
            Settings settings = Settings.Load();
            string war3Exe = Path.Combine(settings.war3root, "_retail_/x86_64/Warcraft III.exe");

            int difficulty = settings.Difficulty;
            string windowMode;
            switch (settings.WindowMode)
            {
                case 0:
                    windowMode = "windowed";
                    break;
                case 1:
                    windowMode = "windowedfullscreen";
                    break;
                default:
                    windowMode = "fullscreen";
                    break;
            }
            int hd = settings.HD;
            int teen = settings.Teen;
            string playerProfile = settings.PlayerProfile;
            int fixedseed = settings.FixedRandomSeed == true ? 1 : 0;
            string nowfpause = settings.NoWindowsFocusPause == true ? "-nowfpause " : "";

            string launchArgs = $"-launch " +
                $"-mapdiff {difficulty} " +
                $"-windowmode {windowMode} " +
                $"-hd {hd} " +
                $"-teen {teen} " +
                $"-testmapprofile {playerProfile} " +
                $"-fixedseed {fixedseed} " +
                $"{nowfpause}";

            Process.Start($"\"{war3Exe}\" {launchArgs} -loadfile \"{fullPath}\"");
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
            for (int i = 0; i < fileCheckList.Count; i++)
            {
                OnCreateElement(fileCheckList[i], false);
            }

            CommandManager.Reset(); // hack, but works. Above OnCreate loop adds commands.

            ControllerReferences controllerRef = new ControllerReferences();
            controllerRef.UpdateReferencesAll();
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

            int insertIndex = 0;
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

                    explorerElementChild.SetParent(elementParent, insertIndex);
                    insertIndex++;
                    if (Directory.Exists(explorerElementChild.GetPath()))
                        RecurseLoad(entryChild, explorerElementChild, fileCheckList);

                }
            }
        }

        public void SaveProject()
        {
            // Write to unsaved
            var unsaved = ContainerUnsavedFiles.GetAllUnsaved();
            for (int i = 0; i < ContainerUnsavedFiles.Count(); i++)
            {
                if (unsaved[i] is IExplorerSaveable)
                {
                    var saveable = (IExplorerSaveable)unsaved[i];
                    File.WriteAllText(unsaved[i].GetPath(), saveable.GetSaveableString());
                }
            }
            ContainerUnsavedFiles.Clear();

            // Write to project file
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
            File.WriteAllText(root.GetProjectPath(), json);
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

        public ExplorerElementRoot GetProjectRoot()
        {
            return (ExplorerElementRoot)ContainerProject.projectFiles[0];
        }

        public void SetElementEnabled(IExplorerElement element, bool isEnabled)
        {
            element.SetEnabled(isEnabled);
        }

        public void SetElementInitiallyOn(IExplorerElement element, bool isInitiallyOn)
        {
            element.SetInitiallyOn(isInitiallyOn);
        }

        /// <summary>
        /// Prevents the system from responding to file changes.
        /// </summary>
        /// <param name="doEnable"></param>
        public void SetEnableFileEvents(bool doEnable)
        {
            ContainerProject.fileSystemWatcher.EnableRaisingEvents = doEnable;
        }

        /// <summary>
        /// Add newly created ExplorerElements to their appropriate container.
        /// </summary>
        /// <param name="createdElement"></param>
        public void AddElementToContainer(IExplorerElement element)
        {
            if (element is ExplorerElementFolder)
                ContainerFolders.AddFolder(element as ExplorerElementFolder);
            else if (element is ExplorerElementTrigger)
                ContainerTriggers.AddTrigger(element as ExplorerElementTrigger);
            else if (element is ExplorerElementScript)
                ContainerScripts.AddScript(element as ExplorerElementScript);
            else if (element is ExplorerElementVariable)
                ContainerVariables.AddVariable(element as ExplorerElementVariable);

        }

        /// <summary>
        /// Removes deleted ExplorerElements from their appropriate container.
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElementFromContainer(IExplorerElement element)
        {
            if (element is ExplorerElementFolder)
                ContainerFolders.Remove(element as ExplorerElementFolder);
            else if (element is ExplorerElementTrigger)
                ContainerTriggers.Remove(element as ExplorerElementTrigger);
            else if (element is ExplorerElementScript)
                ContainerScripts.Remove(element as ExplorerElementScript);
            else if (element is ExplorerElementVariable)
                ContainerVariables.Remove(element as ExplorerElementVariable);
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

            switch (Path.GetExtension(fullPath))
            {
                case "":
                    explorerElement = new ExplorerElementFolder(fullPath);
                    break;
                case ".trg":
                    explorerElement = new ExplorerElementTrigger(fullPath);
                    break;
                case ".j":
                    explorerElement = new ExplorerElementScript(fullPath);
                    break;
                case ".var":
                    explorerElement = new ExplorerElementVariable(fullPath);
                    break;
                default:
                    break;
            }
            AddElementToContainer(explorerElement);

            CommandExplorerElementCreate command = new CommandExplorerElementCreate(explorerElement, parent, parent.GetExplorerElements().Count);
            command.Execute();

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
        /// This is used when we want to redo a 'create file' action
        /// or undo a 'delete' action.
        /// </summary>
        public void RecurseCreateElementsWithContent(IExplorerElement topElement, bool doRecurse = true)
        {
            if (topElement is IExplorerSaveable)
            {
                var element = (IExplorerSaveable)topElement;
                File.WriteAllText(topElement.GetPath(), element.GetSaveableString());
            }
            else
                Directory.CreateDirectory(topElement.GetPath());

            topElement.UpdateMetadata(); // important, because this is a pseudo-redo

            if (Directory.Exists(topElement.GetPath()))
            {
                var folder = (ExplorerElementFolder)topElement;
                if (!doRecurse)
                    return;

                for (int i = 0; i < folder.explorerElements.Count; i++)
                {
                    var element = folder.explorerElements[i];
                    RecurseCreateElementsWithContent(element);
                }
            }
        }

        public void OnRenameElement(string oldFullPath, string newFullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);

            CommandExplorerElementRename command = new CommandExplorerElementRename(elementToRename, newFullPath);
            command.Execute();
        }

        /// <summary>
        /// This is also used when files are moved, hence why we need
        /// to detach it from its current parent and attach it to the other.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        public void OnMoveElement(string oldFullPath, string newFullPath, int insertIndex)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);

            CommandExplorerElementMove command = new CommandExplorerElementMove(elementToRename, newFullPath, insertIndex);
            command.Execute();
        }

        /// <summary>
        /// Needed when a folder containing files is renamed. 
        /// </summary>
        /// <param name="elementToRename"></param>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        internal void RecurseMoveElement(IExplorerElement elementToRename, string oldFullPath, string newFullPath)
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
                        RecurseMoveElement(elementInSubSearch, elementToRename.GetPath(), entries[i]);
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
            RemoveElementFromContainer(elementToDelete);

            CommandExplorerElementDelete command = new CommandExplorerElementDelete(elementToDelete);
            command.Execute();
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
            {
                elementToChange.UpdateMetadata();
                elementToChange.Notify();
            }
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

        public bool WasFileMoved(string oldFullPath)
        {
            var explorerElement = FindExplorerElement(ContainerProject.projectFiles[0], oldFullPath);
            var exPath = explorerElement.GetPath();

            bool wasMoved = false;
            var files = Directory.GetFileSystemEntries(Path.GetDirectoryName(ContainerProject.projectFiles[0].GetPath()), "*", SearchOption.AllDirectories);
            int i = 0;
            while (i < files.Length && !wasMoved)
            {
                if (Path.GetFileName(exPath) == Path.GetFileName(files[i]))
                {
                    long size = 0;
                    DateTime lastWrite = new DateTime(0);
                    if (File.Exists(files[i]))
                    {
                        size = new FileInfo(files[i]).Length;
                        lastWrite = new FileInfo(files[i]).LastWriteTime;
                    }
                    else if (Directory.Exists(files[i]))
                    {
                        var info = new DirectoryInfo(files[i]);
                        size = info.EnumerateFiles().Sum(file => file.Length);
                        lastWrite = new DirectoryInfo(files[i]).LastWriteTime;
                    }
                    if (size == explorerElement.GetSize() && lastWrite == explorerElement.GetLastWrite())
                        wasMoved = true;
                }

                i++;
            }

            return wasMoved;
        }

        /// <summary>
        /// Determines whether an ExplorerElement with the given name exists or not.
        /// </summary>
        /// <param name="explorerElement"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool DoesNameExist(IExplorerElement explorerElement, string newName)
        {
            bool exists = false;

            if (explorerElement is ExplorerElementTrigger)
                exists = ContainerTriggers.Contains(newName);
            else if (explorerElement is ExplorerElementScript)
                exists = ContainerScripts.Contains(newName);
            else if (explorerElement is ExplorerElementVariable)
                exists = ContainerVariables.Contains(newName);
            else if (explorerElement is ExplorerElementFolder)
            {
                string parentDir = explorerElement.GetParent().GetPath();
                string[] files = Directory.GetFileSystemEntries(parentDir, "*", SearchOption.TopDirectoryOnly);
                int i = 0;
                while (!exists && i < files.Length)
                {
                    if (Directory.Exists(files[i]) && files[i] == Path.Combine(parentDir, newName))
                    {
                        exists = true;
                    }
                    i++;
                }
            }

            return exists;
        }

        public void RenameElement(IExplorerElement explorerElement, string renameText)
        {
            string formattedName = string.Empty;
            if (explorerElement is ExplorerElementFolder)
                formattedName = renameText;
            else if (explorerElement is ExplorerElementTrigger)
                formattedName = renameText + ".trg";
            else if (explorerElement is ExplorerElementScript)
                formattedName = renameText + ".j";
            else if (explorerElement is ExplorerElementVariable)
                formattedName = renameText + ".var";

            ContainerProject.insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);
            ControllerFileSystem controller = new ControllerFileSystem();
            controller.RenameElement(explorerElement.GetPath(), formattedName);
        }

        public IExplorerElement GetCopiedElement()
        {
            return ContainerCopiedElements.CopiedExplorerElement;
        }

        public void CopyExplorerElement(IExplorerElement explorerElement, bool isCut = false)
        {
            IExplorerElement copied = explorerElement.Clone();
            ContainerCopiedElements.CopiedExplorerElement = copied;

            if (isCut)
                ContainerCopiedElements.CutExplorerElement = explorerElement;
            else
                ContainerCopiedElements.CutExplorerElement = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pasteTarget"></param>
        /// <param name="insertIndex"></param>
        /// <returns>The pasted ExplorerElement.</returns>
        public IExplorerElement PasteExplorerElement(IExplorerElement pasteTarget)
        {
            int insertIndex;
            IExplorerElement parent = null;
            if (pasteTarget is ExplorerElementFolder || pasteTarget is ExplorerElementRoot)
            {
                parent = pasteTarget;
                insertIndex = 0;
            }
            else
            {
                parent = pasteTarget.GetParent();
                insertIndex = parent.GetExplorerElements().IndexOf(pasteTarget);
            }

            var pasted = ContainerCopiedElements.CopiedExplorerElement.Clone();
            if (ContainerCopiedElements.CutExplorerElement == null)
                PrepareExplorerElement(pasted);

            CommandExplorerElementPaste command = new CommandExplorerElementPaste(pasted, parent, insertIndex);
            command.Execute();

            return pasted;
        }

        /// <summary>
        /// Adjusts the name and id of ExplorerElement(s) so they don't get an name/id that's already in use.
        /// Use when new elements are about to get created or pasted.
        /// </summary>
        /// <param name="explorerElement"></param>
        public void PrepareExplorerElement(IExplorerElement explorerElement)
        {
            if (explorerElement is ExplorerElementTrigger)
            {
                ControllerTrigger controllerTrigger = new ControllerTrigger();
                var element = (ExplorerElementTrigger)explorerElement;

                string folder = Path.GetDirectoryName(element.GetPath());
                string name = controllerTrigger.GenerateTriggerName();

                element.trigger.Id = ContainerTriggers.GenerateId();
                element.SetPath(Path.Combine(folder, name));
            }
            else if (explorerElement is ExplorerElementVariable)
            {
                var element = (ExplorerElementVariable)explorerElement;
                element.variable.Id = ContainerVariables.GenerateId();
            }
            else if (explorerElement is ExplorerElementFolder)
            {
                var children = explorerElement.GetExplorerElements();
                for (int i = 0; i < children.Count; i++)
                {
                    PrepareExplorerElement(children[i]);
                }
            }
        }
    }
}
