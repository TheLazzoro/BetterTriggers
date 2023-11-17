using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using Cake.Incubator.Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using War3Net.Build.Info;

namespace BetterTriggers.Containers
{
    public class Project
    {
        public static Project CurrentProject { get; private set; }

        public string src;
        public War3Project war3project;
        public List<IExplorerElement> projectFiles;
        public string currentSelectedElement;
        public BufferingFileSystemWatcher fileSystemWatcher;
        public IExplorerElement lastCreated;

        public Folders Folders { get; private set; }
        public Variables Variables { get; private set; }
        public Triggers Triggers { get; private set; }
        public Scripts Scripts { get; private set; }
        public References References { get; private set; }
        public UnsavedFiles UnsavedFiles { get; private set; }
        public CommandManager CommandManager { get; private set; }
        
        public event FileSystemEventHandler OnCreated;
        public event FileSystemEventHandler OnMoved;
        public event FileSystemEventHandler OnDeleted;
        public string createdPath = string.Empty;
        public string deletedPath = string.Empty;
        public int insertIndex = 0;

        bool wasMoved;


        private Project()
        {
            Folders = new();
            Variables = new();
            Triggers = new();
            Scripts = new();
            References = new();
            UnsavedFiles = new();
            CommandManager = new();
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

        /// <summary>
        /// Saves project and all unsaved files.
        /// </summary>
        public void Save()
        {
            if (projectFiles == null)
                return;

            EnableFileEvents(false);

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
            var root = (ExplorerElementRoot)projectFiles[0];
            war3project.Files = new List<War3ProjectFileEntry>();

            for (int i = 0; i < root.explorerElements.Count; i++)
            {
                var element = root.explorerElements[i];

                var fileEntry = new War3ProjectFileEntry();
                fileEntry.path = element.GetSaveablePath();
                fileEntry.isEnabled = element.GetEnabled();
                fileEntry.isInitiallyOn = element.GetInitiallyOn();

                war3project.Files.Add(fileEntry);

                if (element is ExplorerElementFolder)
                {
                    RecurseSaveFileEntries((ExplorerElementFolder)element, fileEntry);
                }
            }

            EnableFileEvents(true);

            var json = JsonConvert.SerializeObject(war3project, Formatting.Indented);
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
            project.war3project = war3project;

            war3project.Version = War3Project.EditorVersion; // updates version.
            war3project.GameVersion = Casc.GameVersion; // updates game version.
            project.src = Path.Combine(Path.GetDirectoryName(projectPath), "src");
            project.war3project = war3project;
            project.projectFiles = new List<IExplorerElement>();
            project.projectFiles.Add(new ExplorerElementRoot(war3project, projectPath));
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
            project.RecurseLoad(projectRootEntry, project.projectFiles[0], fileCheckList);

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
                    if (Directory.Exists(path))
                    {
                        explorerElementChild = new ExplorerElementFolder(path);
                    }
                    else
                    {
                        switch (Path.GetExtension(entryChild.path))
                        {
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
                                continue;
                        }
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


        public void OnCreateElement(string fullPath, bool doRecurse = true)
        {
            string directory = Path.GetDirectoryName(fullPath);

            ExplorerElementRoot root = projectFiles[0] as ExplorerElementRoot;
            IExplorerElement parent = FindExplorerElementFolder(root, directory);
            if(parent == null)
            {
                parent = root;
            }

            RecurseCreateElement(parent, fullPath, doRecurse);
        }

        private void RecurseCreateElement(IExplorerElement parent, string fullPath, bool doRecurse)
        {
            IExplorerElement explorerElement = null;
            if (Directory.Exists(fullPath))
            {
                explorerElement = new ExplorerElementFolder(fullPath);
            }
            else
            {
                switch (Path.GetExtension(fullPath))
                {
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
                        return;
                }
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
                    RecurseCreateElement((ExplorerElementFolder)explorerElement, entries[i], doRecurse);
                }
            }
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

            insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);
            FileSystemUtil.Rename(oldPath, formattedName);
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



        public IExplorerElement OnRenameElement(string oldFullPath, string newFullPath)
        {
            var rootNode = projectFiles[0];
            IExplorerElement elementToRename = FindExplorerElement(rootNode, oldFullPath);
            if (elementToRename == null)
                return null;

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
            var rootNode = projectFiles[0];
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
            var rootNode = projectFiles[0];
            IExplorerElement elementToDelete = FindExplorerElement(rootNode, fullPath);
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
        public void RearrangeElement(IExplorerElement element, int insertIndex)
        {
            CommandExplorerElementMoveEx commandEx = new CommandExplorerElementMoveEx(element, insertIndex);
            commandEx.Execute();
        }

        public void OnElementChanged(string fullPath)
        {
            var rootNode = projectFiles[0];
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


        public IExplorerElement? FindExplorerElementFolder(IExplorerElement parent, string directory)
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
                Variables.AddVariable(variable);
            }

        }

        /// <summary>
        /// Removes deleted ExplorerElements from their appropriate container.
        /// </summary>
        /// <param name="element"></param>
        public void RemoveElementFromContainer(IExplorerElement element)
        {
            if (element is ExplorerElementFolder folder)
            {
                for (int i = 0; i < folder.GetExplorerElements().Count; i++)
                {
                    var subElement = folder.GetExplorerElements()[i];
                    RemoveElementFromContainer(subElement);
                }
                Folders.Remove(folder);
            }
            else if (element is ExplorerElementTrigger trigger)
                Triggers.Remove(trigger);
            else if (element is ExplorerElementScript script)
                Scripts.Remove(script);
            else if (element is ExplorerElementVariable variable)
                Variables.Remove(variable);
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
            lastCreated = pasted;

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
                var element = (ExplorerElementTrigger)explorerElement;

                string folder = Path.GetDirectoryName(element.GetPath());
                string name = Triggers.GenerateTriggerName(explorerElement.GetName());
                element.trigger.Id = Triggers.GenerateId();
                element.SetPath(Path.Combine(folder, name));


                // Adjusts local variable ids
                List<int> blacklistedIds = new List<int>();
                var varRefs = Triggers.GetVariableRefsFromTrigger(element);
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
                var element = (ExplorerElementVariable)explorerElement;

                string folder = Path.GetDirectoryName(element.GetPath());
                string name = Variables.GenerateName(explorerElement.GetName());

                element.variable.Id = Variables.GenerateId();
                element.SetPath(Path.Combine(folder, name + ".var"));

            }
            else if (explorerElement is ExplorerElementFolder)
            {
                var children = explorerElement.GetExplorerElements();
                for (int i = 0; i < children.Count; i++)
                {
                    PrepareExplorerElement(children[i]);
                }
            }

            AddElementToContainer(explorerElement);
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
        public ExplorerElementRoot GetRoot()
        {
            return (ExplorerElementRoot)projectFiles[0];
        }

        public string GetFullMapPath()
        {
            string path = war3project.War3MapDirectory;
            if (war3project.UseRelativeMapDirectory)
            {
                string mapFileName = Path.GetFileName(war3project.War3MapDirectory);
                var root = GetRoot();
                string rootDir = Path.GetDirectoryName(root.GetPath());
                path = Path.Combine(rootDir, "map/" + mapFileName);
            }

            return path;
        }

        public static bool VerifyMapPath(string path)
        {
            if(CurrentProject != null)
            {
                bool useRelativeMapDir = CurrentProject.war3project.UseRelativeMapDirectory;
                if (useRelativeMapDir)
                {
                    path = CurrentProject.GetFullMapPath();
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
                    if (size == explorerElement.GetSize() && lastWrite == explorerElement.GetLastWrite())
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
