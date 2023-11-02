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
            string path = Project.GetFullMapPath();
            if (path != null && Directory.Exists(path) && File.Exists(Path.Combine(path, "war3map.w3i")))
                exists = true;
            else if (path != null && File.Exists(path) && (Path.HasExtension(".w3x") || Path.HasExtension(".w3m")))
                exists = true;

            return exists;
        }

        public static bool VerifyMapPath(string path)
        {
            bool useRelativeMapDir = Project.project.UseRelativeMapDirectory;
            if (useRelativeMapDir)
            {
                path = Project.GetFullMapPath();
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

        // TODO: Why do we use two paths?
        public void SetWar3MapPath(string path)
        {
            Project.project.War3MapDirectory = path;
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
            Project container = new Project();
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
            RecurseLoad(projectRootEntry, Project.projectFiles[0], fileCheckList);

            // Loop through elements not found
            for (int i = 0; i < fileCheckList.Count; i++)
            {
                OnCreateElement(fileCheckList[i], false);
            }

            CommandManager.Reset(); // hack, but works. Above OnCreate loop adds commands.

            References.UpdateReferencesAll();
            ControllerRecentFiles.AddProjectToRecent(filepath);

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

        public void SaveProject()
        {
            if (Project.projectFiles == null)
                return;

            Project.EnableFileEvents(false);

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
            var root = (ExplorerElementRoot)Project.projectFiles[0];
            var project = Project.project;
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

            Project.EnableFileEvents(true);

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
                Variables.Remove(variable);
            }
        }

        public void OnCreateElement(string fullPath, bool doRecurse = true)
        {
            string directory = Path.GetDirectoryName(fullPath);

            ExplorerElementRoot root = Project.projectFiles[0] as ExplorerElementRoot;
            IExplorerElement parent = FindExplorerElementFolder(root, directory);

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
            Project.lastCreated = explorerElement;

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
            var rootNode = Project.projectFiles[0];
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
            var rootNode = Project.projectFiles[0];
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
            var rootNode = Project.projectFiles[0];
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
            var rootNode = Project.projectFiles[0];
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
                return Project.projectFiles[0];

            return matching;
        }

        public bool WasFileMoved(string oldFullPath)
        {
            var explorerElement = FindExplorerElement(Project.projectFiles[0], oldFullPath);
            if (explorerElement == null)
                return false;

            var exPath = explorerElement.GetPath();

            bool wasMoved = false;
            var files = Directory.GetFileSystemEntries(Path.GetDirectoryName(Project.projectFiles[0].GetPath()), "*", SearchOption.AllDirectories);
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

            Project.insertIndex = explorerElement.GetParent().GetExplorerElements().IndexOf(explorerElement);
            ControllerFileSystem.Rename(oldPath, formattedName);
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
            Project.lastCreated = pasted;

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
                string name = ControllerTrigger.GenerateTriggerName(explorerElement.GetName());
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
                var element = (ExplorerElementVariable)explorerElement;

                string folder = Path.GetDirectoryName(element.GetPath());
                string name = ControllerVariable.GenerateName(explorerElement.GetName());

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

    }
}
