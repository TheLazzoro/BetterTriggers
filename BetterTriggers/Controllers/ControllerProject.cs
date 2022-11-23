using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using War3Net.Build;
using War3Net.Build.Info;
using War3Net.IO.Mpq;

namespace BetterTriggers.Controllers
{
    public class ControllerProject
    {
        /// <summary>
        /// </summary>
        /// <returns>Path to the project file.</returns>
        public string CreateProject(ScriptLanguage language, string name, string destinationFolder, bool doCreateMap = true)
        {
            string root = Path.Combine(destinationFolder, name);
            string src = Path.Combine(root, "src");
            string map = Path.Combine(root, "map");
            string dist = Path.Combine(root, "dist");
            string projectPath = Path.Combine(root, name + ".json");
            string mapFolder = Path.Combine(map, "Map.w3x");

            War3Project project = new War3Project()
            {
                Name = name,
                Language = language == ScriptLanguage.Jass ? "jass" : "lua",
                Header = "",
                Files = new List<War3ProjectFileEntry>(),
                War3MapDirectory = mapFolder
            };

            string projectFile = JsonConvert.SerializeObject(project);

            Directory.CreateDirectory(root);
            Directory.CreateDirectory(src);
            Directory.CreateDirectory(map);
            Directory.CreateDirectory(dist);
            File.WriteAllText(projectPath, projectFile);

            // template map
            if (doCreateMap)
            {
                Directory.CreateDirectory(mapFolder);
                string templateFolder = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Resources/MapTemplate");
                string[] files = Directory.GetFiles(templateFolder);
                foreach (var file in files)
                {
                    byte[] content = File.ReadAllBytes(file);
                    string filename = Path.GetFileName(file);
                    File.WriteAllBytes(Path.Combine(mapFolder, filename), content);
                }
            }

            return projectPath;
        }

        public int GetUnsavedFileCount()
        {
            return UnsavedFiles.Count();
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

        public bool GenerateScript()
        {
            War3Project project = ContainerProject.project;
            if (project == null)
                return false;

            ScriptLanguage language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;

            ScriptGenerator scriptGenerator = new ScriptGenerator(language);
            bool success = scriptGenerator.GenerateScript();

            return success;
        }

        string archivePath;
        /// <summary>
        /// Builds an MPQ archive.
        /// </summary>
        /// <returns>Full path of the archive.</returns>
        public bool BuildMap(string destinationDir = null)
        {
            bool wasVerified = GenerateScript();
            if (!wasVerified)
                return false;

            War3Project project = ContainerProject.project;
            ScriptLanguage language = project.Language == "lua" ? ScriptLanguage.Lua : ScriptLanguage.Jass;

            string mapDir = project.War3MapDirectory;
            var map = Map.Open(mapDir);
            map.Info.ScriptLanguage = language;

            MapBuilder builder = new MapBuilder(map);
            builder.AddFiles(mapDir, "*", SearchOption.AllDirectories);
            var archiveCreateOptions = new MpqArchiveCreateOptions
            {
                ListFileCreateMode = MpqFileCreateMode.Overwrite,
                AttributesCreateMode = MpqFileCreateMode.Prune,
                //BlockSize = 3,
            };

            string src = Path.GetDirectoryName(ContainerProject.src);
            if (destinationDir == null)
                archivePath = Path.Combine(src, Path.Combine("dist", Path.GetFileName(mapDir)));
            else
            {
                Settings settings = Settings.Load();
                archivePath = Path.Combine(destinationDir, settings.CopyLocation + ".w3x");
            }

            bool didWrite = false;
            int attemptLimit = 1000;
            while (attemptLimit > 0 && !didWrite)
            {
                try
                {
                    builder.Build(archivePath, archiveCreateOptions);
                    didWrite = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(5);
                    attemptLimit--;
                }
            }
            if (!didWrite)
                throw new Exception("Could not write to map.");


            return true;
        }

        public void TestMap()
        {
            string destinationDir = Path.GetTempPath();
            bool success = BuildMap(destinationDir);
            if (!success)
                return;

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

            Process.Start($"\"{war3Exe}\" {launchArgs} -loadfile \"{archivePath}\"");
        }


        private string src;
        public event Action<int, int> FileLoadEvent;
        private int totalFiles;
        private int loadedFiles;
        /// <summary>
        /// Loads all files from a given project into the container
        /// </summary>
        public War3Project LoadProject(string filepath)
        {
            ControllerRecentFiles controllerRecentFiles = new ControllerRecentFiles();

            if (!File.Exists(filepath))
                throw new Exception($"File '{filepath}' does not exist.");

            string json = File.ReadAllText(filepath);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(json);
            if (project == null)
                throw new Exception($"File '{filepath}' does not exist.");

            if (project.Name == null)
                throw new Exception("Not a valid project file.");

            if (project.Version > War3Project.EditorVersion)
                throw new Exception($"Project failed to load. Requires newer editor version.\n\nProject version: {project.Version}\nEditor version: {War3Project.EditorVersion}");

            if (project.GameVersion > Casc.GameVersion)
                throw new Exception($"Project failed to load. Requires newer game version.\n\nProject version: {project.GameVersion}\nGame version: {Casc.GameVersion}");


            project.Version = War3Project.EditorVersion; // updates version.
            project.GameVersion = Casc.GameVersion; // updates game version.
            src = Path.Combine(Path.GetDirectoryName(filepath), "src");
            ContainerProject container = new ContainerProject();
            container.LoadProject(project, filepath, src);
            var projectRootEntry = new War3ProjectFileEntry()
            {
                isEnabled = true,
                isInitiallyOn = true,
                path = "",
                Files = project.Files,
            };

            // Clear containers.
            Folders.Clear();
            Triggers.Clear();
            Scripts.Clear();
            Variables.Clear();

            // get all files
            string[] files = Directory.GetFileSystemEntries(src, "*", SearchOption.AllDirectories);
            totalFiles = files.Length;
            loadedFiles = 0;
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
                folder.isExpanded = entryParent.isExpanded;
            }

            int insertIndex = 0;
            for (int i = 0; i < entryParent.Files.Count; i++)
            {
                var entryChild = entryParent.Files[i];
                string path = Path.Combine(src, entryChild.path);
                IExplorerElement explorerElementChild = null;

                if (File.Exists(path) || Directory.Exists(path))
                {
                    fileCheckList.Remove(path);

                    // Add item to appropriate container
                    switch (Path.GetExtension(entryChild.path))
                    {
                        case "":
                            explorerElementChild = new ExplorerElementFolder(path);
                            break;
                        case ".trg":
                            explorerElementChild = new ExplorerElementTrigger(path);
                            break;
                        case ".j":
                        case ".lua":
                            explorerElementChild = new ExplorerElementScript(path);
                            break;
                        case ".var":
                            explorerElementChild = new ExplorerElementVariable(path);
                            break;
                        default:
                            break;
                    }

                    explorerElementChild.SetEnabled(entryChild.isEnabled);
                    explorerElementChild.SetInitiallyOn(entryChild.isInitiallyOn);
                    explorerElementChild.SetParent(elementParent, insertIndex);
                    insertIndex++;
                    if (Directory.Exists(explorerElementChild.GetPath()))
                        RecurseLoad(entryChild, explorerElementChild, fileCheckList);

                    loadedFiles++;
                    FileLoadEvent?.Invoke(loadedFiles, totalFiles);
                }
            }
        }

        public void SaveProject()
        {
            if (ContainerProject.projectFiles == null)
                return;

            SetEnableFileEvents(false);

            // Write to unsaved
            var unsaved = UnsavedFiles.GetAllUnsaved();
            for (int i = 0; i < UnsavedFiles.Count(); i++)
            {
                if (unsaved[i] is IExplorerSaveable)
                {
                    var saveable = (IExplorerSaveable)unsaved[i];
                    if (File.Exists(unsaved[i].GetPath())) // Edge case when a folder containing the file was deleted.
                    {
                        File.WriteAllText(unsaved[i].GetPath(), saveable.GetSaveableString());
                        saveable.OnSaved();
                    }
                }
            }
            UnsavedFiles.Clear();

            // Write to project file
            var root = (ExplorerElementRoot)ContainerProject.projectFiles[0];
            var project = ContainerProject.project;
            project.Files = new List<War3ProjectFileEntry>();

            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                var element = root.explorerElements[i];

                var fileEntry = new War3ProjectFileEntry();
                fileEntry.path = element.GetSaveablePath();
                fileEntry.isEnabled = element.GetEnabled();
                fileEntry.isInitiallyOn = element.GetInitiallyOn();

                project.Files.Add(fileEntry);

                if (element is ExplorerElementFolder)
                {
                    RecurseSaveFileEntries((ExplorerElementFolder)element, fileEntry);
                }
            }

            SetEnableFileEvents(true);

            var json = JsonConvert.SerializeObject(project, Formatting.Indented);
            File.WriteAllText(root.GetProjectPath(), json);
        }

        private void RecurseSaveFileEntries(ExplorerElementFolder parent, War3ProjectFileEntry parentEntry)
        {
            List<IExplorerElement> children = parent.explorerElements;
            parentEntry.isExpanded = parent.isExpanded;

            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];

                // TODO: DUPLICATE CODE!!
                var fileEntryChild = new War3ProjectFileEntry();
                fileEntryChild.path = element.GetSaveablePath();
                fileEntryChild.isEnabled = element.GetEnabled();
                fileEntryChild.isInitiallyOn = element.GetInitiallyOn();

                parentEntry.Files.Add(fileEntryChild);

                if (element is ExplorerElementFolder)
                {
                    RecurseSaveFileEntries((ExplorerElementFolder)element, fileEntryChild);
                }
            }
        }

        public void CloseProject()
        {
            SetEnableFileEvents(false);
            ContainerProject.project = null;
            ContainerProject.projectFiles = null;
            ContainerProject.currentSelectedElement = null;

            CustomMapData.mapPath = null;

            CommandManager.Reset();
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
                Folders.AddFolder(element as ExplorerElementFolder);
            else if (element is ExplorerElementTrigger)
                Triggers.AddTrigger(element as ExplorerElementTrigger);
            else if (element is ExplorerElementScript)
                Scripts.AddScript(element as ExplorerElementScript);
            else if (element is ExplorerElementVariable)
            {
                var variable = (ExplorerElementVariable)element;
                Variables.AddVariable(variable.variable);
            }

        }

        /// <summary>
        /// Removes deleted ExplorerElements from their appropriate container.
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElementFromContainer(IExplorerElement element)
        {
            if (element is ExplorerElementFolder)
            {
                var folder = element as ExplorerElementFolder;
                for (int i = 0; i < element.GetExplorerElements().Count; i++)
                {
                    var subElement = element.GetExplorerElements()[i];
                    RemoveElementFromContainer(subElement);
                }
                Folders.Remove(element as ExplorerElementFolder);
            }
            else if (element is ExplorerElementTrigger)
                Triggers.Remove(element as ExplorerElementTrigger);
            else if (element is ExplorerElementScript)
                Scripts.Remove(element as ExplorerElementScript);
            else if (element is ExplorerElementVariable)
            {
                var variable = (ExplorerElementVariable)element;
                Variables.Remove(variable.variable);
            }
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
                case ".lua":
                    explorerElement = new ExplorerElementScript(fullPath);
                    break;
                case ".var":
                    explorerElement = new ExplorerElementVariable(fullPath);
                    break;
                default:
                    break;
            }
            AddElementToContainer(explorerElement);
            ContainerProject.lastCreated = explorerElement;

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

        public IExplorerElement OnRenameElement(string oldFullPath, string newFullPath)
        {
            var rootNode = ContainerProject.projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);

            CommandExplorerElementRename command = new CommandExplorerElementRename(elementToRename, newFullPath);
            command.Execute();

            return elementToRename;
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
            CommandExplorerElementMoveEx commandEx = new CommandExplorerElementMoveEx(element, insertIndex);
            commandEx.Execute();
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

        public void RenameElement(IExplorerElement explorerElement, string renameText)
        {
            string oldPath = explorerElement.GetPath();

            string formattedName = string.Empty;
            if (explorerElement is ExplorerElementFolder)
                formattedName = renameText;
            else if (explorerElement is ExplorerElementTrigger)
            {
                if (Triggers.Contains(renameText))
                    throw new Exception($"Trigger '{renameText}' already exists.");

                formattedName = renameText + ".trg";
            }
            else if (explorerElement is ExplorerElementScript)
                formattedName = renameText + ".j";
            else if (explorerElement is ExplorerElementVariable)
            {
                if (Variables.Contains(renameText))
                    throw new Exception($"Variable '{renameText}' already exists.");

                formattedName = renameText + ".var";
            }

            ContainerProject.insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);
            ControllerFileSystem controller = new ControllerFileSystem();
            controller.RenameElement(oldPath, formattedName);
        }

        public IExplorerElement GetCopiedElement()
        {
            return CopiedElements.CopiedExplorerElement;
        }

        public void CopyExplorerElement(IExplorerElement explorerElement, bool isCut = false)
        {
            IExplorerElement copied = explorerElement.Clone();
            if (copied == null)
                return;

            CopiedElements.CopiedExplorerElement = copied;

            if (isCut)
                CopiedElements.CutExplorerElement = explorerElement;
            else
                CopiedElements.CutExplorerElement = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pasteTarget">Currently selected element when pasting.</param>
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

            var pasted = CopiedElements.CopiedExplorerElement.Clone();
            if (CopiedElements.CutExplorerElement == null)
                PrepareExplorerElement(pasted);

            CommandExplorerElementPaste command = new CommandExplorerElementPaste(pasted, parent, insertIndex);
            command.Execute();
            ContainerProject.lastCreated = pasted;

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

                element.trigger.Id = Triggers.GenerateId();
                element.SetPath(Path.Combine(folder, name));


                // Adjusts local variable ids
                List<int> blacklistedIds = new List<int>();
                ControllerTrigger controller = new ControllerTrigger();
                var varRefs = controller.GetVariableRefsFromTrigger(element);
                element.trigger.LocalVariables.ForEach(v =>
                {
                    var lv = (LocalVariable)v;
                    int oldId = lv.variable.Id;
                    int newId = Variables.GenerateId(blacklistedIds);
                    Variables.AddLocalVariable(lv);
                    lv.variable.Id = newId;
                    blacklistedIds.Add(newId);

                    var matches = varRefs.Where(x => x.VariableId == oldId);
                    foreach (var match in matches)
                        match.VariableId = newId;
                });

            }
            else if (explorerElement is ExplorerElementVariable)
            {
                ControllerVariable controllerVariable = new ControllerVariable();
                var element = (ExplorerElementVariable)explorerElement;

                string folder = Path.GetDirectoryName(element.GetPath());
                string name = controllerVariable.GenerateName();

                element.variable.Id = Variables.GenerateId();
                element.SetPath(Path.Combine(folder, name));
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
