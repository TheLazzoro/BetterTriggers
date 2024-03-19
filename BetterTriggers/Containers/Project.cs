using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using War3Net.Build.Info;

namespace BetterTriggers.Containers
{
    public class Project
    {
        public static Project CurrentProject { get; private set; }

        public string src;
        public string MapName { get; private set; }
        public string ProjectPath { get; private set; }
        public War3Project war3project;
        public ObservableCollection<ExplorerElement> projectFiles;
        public string currentSelectedElement;
        public BufferingFileSystemWatcher fileSystemWatcher;
        public ExplorerElement lastCreated;

        public Folders Folders { get; private set; }
        public Variables Variables { get; private set; }
        public Triggers Triggers { get; private set; }
        public Scripts Scripts { get; private set; }
        public References References { get; private set; }
        public UnsavedFiles UnsavedFiles { get; private set; }
        public CommandManager CommandManager { get; private set; }
        public Dictionary<string, ExplorerElement> AllElements { get; set; }

        public bool IsLoading;
        public event FileSystemEventHandler OnCreated;
        public event FileSystemEventHandler OnMoved;
        public event FileSystemEventHandler OnDeleted;
        public string createdPath = string.Empty;
        public string deletedPath = string.Empty;
        public int insertIndex = 0;

        bool wasMoved;


        private Project()
        {
            IsLoading = true;
            Folders = new();
            Variables = new();
            Triggers = new();
            Scripts = new();
            References = new();
            UnsavedFiles = new();
            CommandManager = new();
            AllElements = new();
        }

        /// <summary>
        /// </summary>
        /// <returns>Path to the project file.</returns>
        public static string Create(ScriptLanguage language, string name, string destinationFolder, bool doCreateMap = true)
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

        /// <summary>
        /// This is used when we want to redo a 'create file' action
        /// or undo a 'delete' action.
        /// </summary>
        public void RecurseCreateElementsWithContent(ExplorerElement topElement, bool doRecurse = true)
        {
            topElement.Save();
            topElement.UpdateMetadata(); // important, because this is a pseudo-redo
            var parent = topElement.GetParent();
            //topElement.Created(parent.GetExplorerElements().IndexOf(topElement)); // THIS CALL USED TO ADD LOCAL VARIABLES TO THEIR CONTAINER!!!!

            if (Directory.Exists(topElement.GetPath()))
            {
                var folder = topElement;
                if (!doRecurse)
                    return;

                for (int i = 0; i < folder.ExplorerElements.Count; i++)
                {
                    var element = folder.ExplorerElements[i];
                    RecurseCreateElementsWithContent(element);
                }
            }
        }

        /// <summary>
        /// Saves project and all unsaved files.
        /// </summary>
        public void Save()
        {
            if (projectFiles == null)
                return;

            EnableFileEvents(false);

            // Write to unsaved
            UnsavedFiles.SaveAll();

            // Write to project file
            var root = projectFiles[0];
            war3project.Files = new List<War3ProjectFileEntry>();

            for (int i = 0; i < root.ExplorerElements.Count; i++)
            {
                var element = root.ExplorerElements[i];

                var fileEntry = new War3ProjectFileEntry();
                fileEntry.path = element.GetRelativePath();
                fileEntry.isEnabled = element.IsEnabled;
                fileEntry.isInitiallyOn = element.IsInitiallyOn;
                fileEntry.isExpanded = element.IsExpanded;

                war3project.Files.Add(fileEntry);

                if (element.ElementType == ExplorerElementEnum.Folder)
                {
                    RecurseSaveFileEntries(element, fileEntry);
                }
            }

            EnableFileEvents(true);

            var json = JsonConvert.SerializeObject(war3project, Formatting.Indented);
            File.WriteAllText(ProjectPath, json);
        }

        private void RecurseSaveFileEntries(ExplorerElement parent, War3ProjectFileEntry parentEntry)
        {
            ObservableCollection<ExplorerElement> children = parent.ExplorerElements;
            parentEntry.isExpanded = parent.IsExpanded;

            for (int i = 0; i < children.Count; i++)
            {
                var element = children[i];

                // TODO: DUPLICATE CODE!!
                var fileEntryChild = new War3ProjectFileEntry();
                fileEntryChild.path = element.GetRelativePath();
                fileEntryChild.isEnabled = element.IsEnabled;
                fileEntryChild.isInitiallyOn = element.IsInitiallyOn;
                fileEntryChild.isExpanded = element.IsExpanded;

                parentEntry.Files.Add(fileEntryChild);

                if (element.ElementType == ExplorerElementEnum.Folder)
                {
                    RecurseSaveFileEntries(element, fileEntryChild);
                }
            }
        }



        public static event Action<int, int> FileLoadEvent;
        public static event Action LoadingUnknownFilesEvent;
        private int totalFiles;
        private int loadedFiles;
        /// <summary>
        /// Loads all files from a given project into the container
        /// </summary>
        public static Project Load(string projectPath)
        {

            if (!File.Exists(projectPath))
                throw new Exception($"File '{projectPath}' does not exist.");

            string json = File.ReadAllText(projectPath);
            War3Project war3project = JsonConvert.DeserializeObject<War3Project>(json);
            if (war3project == null)
                throw new Exception($"File '{projectPath}' does not exist.");

            if (war3project.Name == null)
                throw new Exception("Not a valid project file.");

            if (war3project.Version > War3Project.EditorVersion)
                throw new Exception($"Project failed to load. Requires newer editor version.\n\nProject version: {war3project.Version}\nEditor version: {War3Project.EditorVersion}");

            if (war3project.GameVersion > Casc.GameVersion)
                throw new Exception($"Project failed to load. Requires newer game version.\n\nProject version: {war3project.GameVersion}\nGame version: {Casc.GameVersion}");


            Project project = new Project();
            CurrentProject = project;
            project.MapName = Path.GetFileNameWithoutExtension(projectPath);
            project.ProjectPath = projectPath;
            project.war3project = war3project;

            war3project.Version = War3Project.EditorVersion; // updates version.
            war3project.GameVersion = Casc.GameVersion; // updates game version.
            project.src = Path.Combine(Path.GetDirectoryName(projectPath), "src");
            project.war3project = war3project;
            project.projectFiles = new();
            project.projectFiles.Add(new ExplorerElement(project.src, ExplorerElementEnum.Root));
            project.currentSelectedElement = project.src; // defaults to here when nothing has been selected yet.

            if (project.fileSystemWatcher == null)
            {
                project.fileSystemWatcher = new BufferingFileSystemWatcher();
                project.fileSystemWatcher.Created += project.FileSystemWatcher_Created;
                project.fileSystemWatcher.Deleted += project.FileSystemWatcher_Deleted;
                project.fileSystemWatcher.Changed += project.FileSystemWatcher_Changed;
                project.fileSystemWatcher.Renamed += project.FileSystemWatcher_Renamed;
                project.fileSystemWatcher.Error += project.FileSystemWatcher_Error;
            }

            project.fileSystemWatcher.Path = project.src;
            project.fileSystemWatcher.EnableRaisingEvents = true;
            project.fileSystemWatcher.IncludeSubdirectories = true;
            project.fileSystemWatcher.InternalBufferSize = 32768; // 32 KB. 64 KB is the limit according to Microsoft.


            var projectRootEntry = new War3ProjectFileEntry()
            {
                isEnabled = true,
                isInitiallyOn = true,
                path = "",
                Files = war3project.Files,
            };

            // get all files
            string[] files = Directory.GetFileSystemEntries(project.src, "*", SearchOption.AllDirectories);
            project.totalFiles = files.Length;
            project.loadedFiles = 0;
            List<string> fileCheckList = new List<string>();
            fileCheckList.AddRange(files);

            // Recurse through elements found in the project file
            project.RecurseLoad(projectRootEntry, project.GetRoot(), fileCheckList);

            // Loop through elements not found
            LoadingUnknownFilesEvent?.Invoke();
            for (int i = 0; i < fileCheckList.Count; i++)
            {
                project.OnCreateElement(fileCheckList[i], false);
                project.loadedFiles++;
                FileLoadEvent?.Invoke(project.loadedFiles, project.totalFiles);
            }

            project.CommandManager.Reset(); // hack, but works. Above OnCreate loop adds commands.

            project.References.UpdateReferencesAll();
            RecentFiles.AddProjectToRecent(projectPath);

            project.IsLoading = false;
            project.GetRoot().IsExpanded = true;

            return project;
        }


        private void RecurseLoad(War3ProjectFileEntry entryParent, ExplorerElement elementParent, List<string> fileCheckList)
        {
            ObservableCollection<ExplorerElement> elementChildren = null;
            if (elementParent.ElementType == ExplorerElementEnum.Root)
            {
                var root = elementParent;
                elementChildren = root.ExplorerElements;
            }
            else if (elementParent.ElementType is ExplorerElementEnum.Folder)
            {
                var folder = elementParent;
                elementChildren = folder.ExplorerElements;
                folder.IsExpanded = entryParent.isExpanded;
            }

            int insertIndex = 0;
            for (int i = 0; i < entryParent.Files.Count; i++)
            {
                var entryChild = entryParent.Files[i];
                string path = Path.Combine(src, entryChild.path);
                if (File.Exists(path) || Directory.Exists(path))
                {
                    fileCheckList.Remove(path);
                    ExplorerElement explorerElementChild = new ExplorerElement(path);
                    explorerElementChild.IsEnabled = entryChild.isEnabled;
                    explorerElementChild.IsInitiallyOn = entryChild.isInitiallyOn;
                    explorerElementChild.SetParent(elementParent, insertIndex);
                    insertIndex++;
                    if (Directory.Exists(explorerElementChild.GetPath()))
                        RecurseLoad(entryChild, explorerElementChild, fileCheckList);

                    loadedFiles++;
                    FileLoadEvent?.Invoke(loadedFiles, totalFiles);
                }
            }
        }


        public void OnCreateElement(string fullPath, bool doRecurse = true)
        {
            string directory = Path.GetDirectoryName(fullPath);

            ExplorerElement root = projectFiles[0];
            ExplorerElement parent = FindExplorerElementFolder(root, directory);
            if (parent == null)
            {
                parent = root;
            }

            RecurseCreateElement(parent, fullPath, doRecurse);
        }

        private void RecurseCreateElement(ExplorerElement parent, string fullPath, bool doRecurse)
        {
            ExplorerElement explorerElement = null;
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                explorerElement = new ExplorerElement(fullPath);
            }
            else
            {
                return;
            }

            AddElementToContainer(explorerElement);
            lastCreated = explorerElement;

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
                    RecurseCreateElement(explorerElement, entries[i], doRecurse);
                }
            }
        }


        public bool War3MapDirExists()
        {
            bool exists = false;
            string path = Project.CurrentProject.GetFullMapPath();
            if (path != null && Directory.Exists(path) && File.Exists(Path.Combine(path, "war3map.w3i")))
                exists = true;
            else if (path != null && File.Exists(path) && (Path.HasExtension(".w3x") || Path.HasExtension(".w3m")))
                exists = true;

            return exists;
        }

        public void OnRenameElement(string oldFullPath, string newFullPath)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var rootNode = projectFiles[0];
                ExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);
                if (elementToRename == null)
                    return;

                CommandExplorerElementRename command = new CommandExplorerElementRename(elementToRename, newFullPath);
                command.Execute();
            });
        }

        /// <summary>
        /// This is also used when files are moved, hence why we need
        /// to detach it from its current parent and attach it to the other.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        public void OnMoveElement(string oldFullPath, string newFullPath, int insertIndex)
        {
            var rootNode = projectFiles[0];
            ExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);

            CommandExplorerElementMove command = new CommandExplorerElementMove(elementToRename, newFullPath, insertIndex);
            command.Execute();
        }

        /// <summary>
        /// Needed when a folder containing files is renamed. 
        /// </summary>
        /// <param name="elementToRename"></param>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        internal void RecurseMoveElement(ExplorerElement elementToRename, string oldFullPath, string newFullPath)
        {
            if (elementToRename != null)
            {
                if (Directory.Exists(newFullPath))
                {
                    var folder = elementToRename;

                    string[] entries = Directory.GetFileSystemEntries(newFullPath);
                    for (int i = 0; i < entries.Length; i++)
                    {
                        ExplorerElement elementInSubSearch = null;
                        string filename = Path.GetFileName(entries[i]);

                        // find element with matching old path
                        for (int e = 0; e < folder.ExplorerElements.Count; e++)
                        {
                            if (filename == Path.GetFileName(folder.ExplorerElements[e].GetPath()))
                            {
                                elementInSubSearch = folder.ExplorerElements[e];
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
            var rootNode = projectFiles[0];
            ExplorerElement elementToDelete = FindExplorerElement(rootNode, fullPath);
            if (elementToDelete == null)
                return;

            RemoveElementFromContainer(elementToDelete);

            CommandExplorerElementDelete command = new CommandExplorerElementDelete(elementToDelete);
            command.Execute();
        }


        /// <summary>
        /// Used when an element is rearranged within it's current folder.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="index"></param>
        public void RearrangeElement(ExplorerElement element, int insertIndex)
        {
            CommandExplorerElementMoveEx commandEx = new CommandExplorerElementMoveEx(element, insertIndex);
            commandEx.Execute();
        }

        public void OnElementChanged(string fullPath)
        {
            var rootNode = projectFiles[0];
            ExplorerElement elementToChange = FindExplorerElement(rootNode, fullPath);

            if (elementToChange != null)
            {
                elementToChange.UpdateMetadata();
                elementToChange.Notify();
            }
        }

        public ExplorerElement FindExplorerElement(ExplorerElement parent, string path)
        {
            ExplorerElement matching = null;
            ObservableCollection<ExplorerElement> children = null;
            if (parent.ElementType == ExplorerElementEnum.Root || parent.ElementType == ExplorerElementEnum.Folder)
            {
                children = parent.ExplorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                ExplorerElement element = children[i];
                if (element.GetPath() == path)
                {
                    matching = element;
                    break;
                }
                if (Directory.Exists(element.GetPath()) && matching == null)
                {
                    matching = FindExplorerElement(element, path);
                }
            }

            return matching;
        }

        public ExplorerElement? FindExplorerElementFolder(ExplorerElement parent, string directory)
        {
            ExplorerElement matching = null;
            ObservableCollection<ExplorerElement> children = null;

            if (parent.ElementType == ExplorerElementEnum.Root || parent.ElementType == ExplorerElementEnum.Folder)
            {
                children = parent.ExplorerElements;
            }

            for (int i = 0; i < children.Count; i++)
            {
                ExplorerElement element = children[i];
                if (Directory.Exists(element.GetPath()) && (matching == null || matching.ElementType == ExplorerElementEnum.Root))
                {
                    if (element.GetPath() == directory)
                    {
                        matching = element;
                        break;
                    }
                    else
                    {
                        matching = FindExplorerElementFolder(element, directory);
                        if (matching != null)
                            break;
                    }
                }
            }

            return matching;
        }

        /// <summary>
        /// Add newly created ExplorerElements to their appropriate container.
        /// </summary>
        /// <param name="createdElement"></param>
        public void AddElementToContainer(ExplorerElement element)
        {
            if (element.ElementType == ExplorerElementEnum.Folder)
                Folders.AddFolder(element);
            else if (element.ElementType == ExplorerElementEnum.Trigger)
                Triggers.AddTrigger(element);
            else if (element.ElementType == ExplorerElementEnum.Script)
                Scripts.AddScript(element);
            else if (element.ElementType == ExplorerElementEnum.GlobalVariable)
                Variables.AddVariable(element);

        }

        /// <summary>
        /// Removes deleted ExplorerElements from their appropriate container.
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElementFromContainer(ExplorerElement element)
        {
            if (element.ElementType == ExplorerElementEnum.Folder)
            {
                for (int i = 0; i < element.GetExplorerElements().Count; i++)
                {
                    var subElement = element.GetExplorerElements()[i];
                    RemoveElementFromContainer(subElement);
                }
                Folders.Remove(element);
            }
            else if (element.ElementType == ExplorerElementEnum.Trigger)
                Triggers.Remove(element);
            else if (element.ElementType == ExplorerElementEnum.Script)
                Scripts.Remove(element);
            else if (element.ElementType == ExplorerElementEnum.GlobalVariable)
                Variables.Remove(element);
        }

        public void CopyExplorerElement(ExplorerElement explorerElement, bool isCut = false)
        {
            ExplorerElement copied = explorerElement.Clone();
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
        public ExplorerElement PasteExplorerElement(ExplorerElement pasteTarget)
        {
            int insertIndex;
            ExplorerElement parent = null;
            if (pasteTarget.ElementType == ExplorerElementEnum.Folder || pasteTarget.ElementType == ExplorerElementEnum.Root)
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
            lastCreated = pasted;

            return pasted;
        }


        /// <summary>
        /// Adjusts the name and id of ExplorerElement(s) so they don't get an name/id that's already in use.
        /// Use when new elements are about to get created or pasted.
        /// </summary>
        /// <param name="pasted"></param>
        public void PrepareExplorerElement(ExplorerElement pasted)
        {
            if (pasted.ElementType == ExplorerElementEnum.Trigger)
            {
                string folder = Path.GetDirectoryName(pasted.GetPath());
                string name = Triggers.GenerateTriggerName(pasted.GetName());
                pasted.trigger.Id = Triggers.GenerateId();
                pasted.SetPath(Path.Combine(folder, name));


                // Adjusts local variable ids
                List<int> blacklistedIds = new List<int>();
                var varRefs = Triggers.GetVariableRefsFromTrigger(pasted);
                pasted.trigger.LocalVariables.Elements.ForEach(v =>
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
            else if (pasted.ElementType == ExplorerElementEnum.GlobalVariable)
            {
                string folder = Path.GetDirectoryName(pasted.GetPath());
                string name = Variables.GenerateName(pasted.GetName());

                pasted.variable.Id = Variables.GenerateId();
                pasted.variable.Name = name;
                pasted.SetPath(Path.Combine(folder, name + ".var"));

            }
            else if (pasted.ElementType == ExplorerElementEnum.Folder)
            {
                var children = pasted.GetExplorerElements();
                for (int i = 0; i < children.Count; i++)
                {
                    PrepareExplorerElement(children[i]);
                }
            }

            AddElementToContainer(pasted);
        }

        /// <summary>
        /// Closes the currently opened project.
        /// </summary>
        public static void Close()
        {
            var project = CurrentProject;
            project.fileSystemWatcher.EnableRaisingEvents = false;
            project.fileSystemWatcher.Created -= project.FileSystemWatcher_Created;
            project.fileSystemWatcher.Deleted -= project.FileSystemWatcher_Deleted;
            project.fileSystemWatcher.Changed -= project.FileSystemWatcher_Changed;
            project.fileSystemWatcher.Renamed -= project.FileSystemWatcher_Renamed;
            project.fileSystemWatcher.Error -= project.FileSystemWatcher_Error;
        }

        /// <returns>The top level explorer element in the project.</returns>
        public ExplorerElement GetRoot()
        {
            return projectFiles[0];
        }

        public string GetFullMapPath(string mapFileName = null)
        {
            string path = war3project.War3MapDirectory;
            if (war3project.UseRelativeMapDirectory)
            {
                if (mapFileName == null)
                {
                    mapFileName = Path.GetFileName(war3project.War3MapDirectory);
                }
                var root = GetRoot();
                string rootDir = Path.GetDirectoryName(root.GetPath());
                path = Path.Combine(rootDir, "map/" + mapFileName);
            }

            return path;
        }

        public static bool VerifyMapPath(string path)
        {
            if (CurrentProject != null)
            {
                bool useRelativeMapDir = CurrentProject.war3project.UseRelativeMapDirectory;
                if (useRelativeMapDir)
                {
                    path = CurrentProject.GetFullMapPath(path);
                }
            }

            bool verified = false;
            if (File.Exists(path))
            {
                if (Path.HasExtension(".w3x") || Path.HasExtension(".w3m"))
                    verified = true;
            }
            else if (Directory.Exists(path))
            {
                if (File.Exists(Path.Combine(path, "war3map.w3i")))
                    verified = true;
            }

            return verified;
        }

        public void SetWar3MapPath(string path)
        {
            CurrentProject.war3project.War3MapDirectory = path;
        }

        public int GetUnsavedFileCount()
        {
            return UnsavedFiles.Count();
        }


        private bool WasFileMoved(string oldFullPath)
        {
            var explorerElement = FindExplorerElement(CurrentProject.projectFiles[0], oldFullPath);
            if (explorerElement == null)
                return false;

            var exPath = explorerElement.GetPath();

            bool wasMoved = false;
            var files = Directory.GetFileSystemEntries(Path.GetDirectoryName(CurrentProject.projectFiles[0].GetPath()), "*", SearchOption.AllDirectories);
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
                    if (size == explorerElement.Size && lastWrite == explorerElement.LastWrite)
                        wasMoved = true;
                }

                i++;
            }

            return wasMoved;
        }


        /// <summary>
        /// Prevents the editor from responding to file changes.
        /// </summary>
        /// <param name="doEnable"></param>
        public void EnableFileEvents(bool doEnable)
        {
            fileSystemWatcher.EnableRaisingEvents = doEnable;
        }

        private void InvokeCreate(object sender, FileSystemEventArgs e)
        {
            // bubble up event
            if (OnCreated != null)
                OnCreated(this, e);
        }

        private void InvokeMove(object sender, FileSystemEventArgs e)
        {
            // bubble up event
            if (OnMoved != null)
                OnMoved(this, e);
        }

        private void InvokeDelete(object sender, FileSystemEventArgs e)
        {
            // bubble up event
            if (OnDeleted != null)
                OnDeleted(this, e);
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            createdPath = e.FullPath;
            if (wasMoved)
            {
                OnMoveElement(deletedPath, createdPath, insertIndex);
                insertIndex = 0; // reset
                wasMoved = false;
            }
            else
            {
                OnCreateElement(createdPath, false);
                InvokeCreate(sender, e);
            }
        }

        [STAThread]
        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (!WasFileMoved(e.FullPath))
            {
                deletedPath = e.FullPath;
                OnDeleteElement(deletedPath);
                InvokeDelete(sender, e);
                wasMoved = false;
            }
            else
            {
                deletedPath = e.FullPath;
                wasMoved = true;
            }
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            OnRenameElement(e.OldFullPath, e.FullPath);
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnElementChanged(e.FullPath);
            }
        }

        private void FileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }
    }
}