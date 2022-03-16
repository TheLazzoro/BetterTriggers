using BetterTriggers.Controllers;
using Model.EditorData;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public class ContainerProject
    {
        public static War3Project project;
        public static List<IExplorerElement> projectFiles;
        public static string currentSelectedElement;
        internal static FileSystemWatcher fileSystemWatcher;

        public static event FileSystemEventHandler OnCreated;
        public static event FileSystemEventHandler OnMoved;
        public static event FileSystemEventHandler OnDeleted;
        static bool wasDeleted;
        static bool wasCreated;
        public static string createdPath = string.Empty;
        public static string deletedPath = string.Empty;
        public static int insertIndex = 0;

        public void NewProject(War3Project project, string path)
        {
            ContainerProject.project = project;
            projectFiles = new List<IExplorerElement>();
            projectFiles.Add(new ExplorerElementRoot(project, path)); // add root folder for safety measures :))
            currentSelectedElement = project.Root; // defaults to here when nothing has been selected yet.

            if (fileSystemWatcher != null)
                fileSystemWatcher.Dispose();

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = project.Root;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            //fileSystemWatcher.Error += FileSystemWatcher_Error;
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
            wasCreated = true;
            createdPath = e.FullPath;
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            wasDeleted = true;
            deletedPath = e.FullPath;
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.OnRenameElement(e.OldFullPath, e.FullPath, insertIndex);
            insertIndex = 0; // reset
        }


        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            if (wasDeleted && wasCreated) // was moved
            {
                ControllerProject controller = new ControllerProject();
                controller.OnRenameElement(deletedPath, createdPath, insertIndex);
                insertIndex = 0; // reset

                InvokeMove(sender, e);
            }
            else if (wasDeleted)
            {
                string path = deletedPath;
                ControllerProject controller = new ControllerProject();
                controller.OnDeleteElement(path);

                InvokeDelete(sender, e);
            }
            else if (wasCreated)
            {
                string path = createdPath;
                ControllerProject controller = new ControllerProject();
                controller.OnCreateElement(path, false);

                InvokeCreate(sender, e);
            }


            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                ControllerProject controller = new ControllerProject();
                controller.OnElementChanged(e.FullPath);
            }

            wasCreated = false;
            wasDeleted = false;
        }

        private void FileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            /*
            MessageBox.Show(e.GetException().Message, "Critical File System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
            */
        }
    }
}
