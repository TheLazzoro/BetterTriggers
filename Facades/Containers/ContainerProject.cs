using Facades.Controllers;
using Model.EditorData;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Facades.Containers
{
    public class ContainerProject
    {
        public static War3Project project;
        public static List<IExplorerElement> projectFiles;
        public static string currentSelectedElement;
        static FileSystemWatcher fileSystemWatcher;

        public static event FileSystemEventHandler OnCreated;
        

        public void NewProject(War3Project project)
        {
            //ContainerFolders.Clear();
            ContainerProject.project = project;
            projectFiles = new List<IExplorerElement>();
            projectFiles.Add(new ExplorerElementRoot(project.Root)); // add root folder for safety measures :))
            currentSelectedElement = project.Root; // defaults to here when nothing has been selected yet.

            if (fileSystemWatcher != null)
                fileSystemWatcher.Dispose();

            fileSystemWatcher = new FileSystemWatcher();
            fileSystemWatcher.Path = project.Root;
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            //fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            //fileSystemWatcher.Error += FileSystemWatcher_Error;
        }

        private void InvokeCreate(object sender, FileSystemEventArgs e)
        {
            // bubble up event
            if (OnCreated != null)
                OnCreated(this, e);
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            ControllerProject controller = new ControllerProject();
            controller.OnCreateElement(path);

            InvokeCreate(sender, e);
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            ControllerProject controller = new ControllerProject();
            controller.OnRenameElement(e.OldFullPath, e.FullPath);

        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string path = e.FullPath;
            ControllerProject controller = new ControllerProject();
            controller.OnDeleteElement(path);
        }


        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            /*
            Application.Current.Dispatcher.Invoke(delegate
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    ControllerTriggerExplorer controller = new ControllerTriggerExplorer();
                    //controller.MoveElement(this, e.OldFullPath, e.FullPath);
                    string s = e.FullPath;
                }
            });
            */
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
