using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace BetterTriggers.Containers
{
    public class Project
    {
        public static string src;
        public static War3Project project;
        public static List<IExplorerElement> projectFiles;
        public static string currentSelectedElement;
        public static BufferingFileSystemWatcher fileSystemWatcher;
        public static IExplorerElement lastCreated;

        public static event FileSystemEventHandler OnCreated;
        public static event FileSystemEventHandler OnMoved;
        public static event FileSystemEventHandler OnDeleted;
        static bool wasMoved;
        public static string createdPath = string.Empty;
        public static string deletedPath = string.Empty;
        public static int insertIndex = 0;

        public void LoadProject(War3Project project, string projectPath, string src)
        {
            Project.src = src;
            Project.project = project;
            projectFiles = new List<IExplorerElement>();
            projectFiles.Add(new ExplorerElementRoot(project, projectPath));
            currentSelectedElement = src; // defaults to here when nothing has been selected yet.

            if (fileSystemWatcher == null)
            {
                fileSystemWatcher = new BufferingFileSystemWatcher();
                fileSystemWatcher.Created += FileSystemWatcher_Created;
                fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
                fileSystemWatcher.Changed += FileSystemWatcher_Changed;
                fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
                fileSystemWatcher.Error += FileSystemWatcher_Error;
            }

            fileSystemWatcher.Path = src;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.InternalBufferSize = 32768; // 32 KB. 64 KB is the limit according to Microsoft.
        }

        public static void Close()
        {
            EnableFileEvents(false);
            project = null;
            projectFiles = null;
            currentSelectedElement = null;
            CommandManager.Reset();
            UnsavedFiles.Clear();
        }

        /// <returns>The top level explorer element in the project.</returns>
        public static ExplorerElementRoot GetRoot()
        {
            return (ExplorerElementRoot)projectFiles[0];
        }

        public static string GetFullMapPath()
        {
            string path = project.War3MapDirectory;
            if (project.UseRelativeMapDirectory)
            {
                string mapFileName = Path.GetFileName(project.War3MapDirectory);
                var root = GetRoot();
                string rootDir = Path.GetDirectoryName(root.GetPath());
                path = Path.Combine(rootDir, "map/" + mapFileName);
            }

            return path;
        }

        /// <summary>
        /// Prevents the system from responding to file changes.
        /// </summary>
        /// <param name="doEnable"></param>
        public static void EnableFileEvents(bool doEnable)
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
            ControllerProject controller = new ControllerProject();
            if (wasMoved)
            {
                controller.OnMoveElement(deletedPath, createdPath, insertIndex);
                insertIndex = 0; // reset
                wasMoved = false;
            }
            else
            {
                controller.OnCreateElement(createdPath, false);
                InvokeCreate(sender, e);
            }
        }

        [STAThread]
        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            if (!controller.WasFileMoved(e.FullPath))
            {
                deletedPath = e.FullPath;
                controller.OnDeleteElement(deletedPath);
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
            ControllerProject controller = new ControllerProject();
            controller.OnRenameElement(e.OldFullPath, e.FullPath);
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                ControllerProject controller = new ControllerProject();
                controller.OnElementChanged(e.FullPath);
            }
        }

        private void FileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            throw new Exception(e.GetException().Message);
        }
    }
}
