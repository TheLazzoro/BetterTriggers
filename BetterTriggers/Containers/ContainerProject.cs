using BetterTriggers.Controllers;
using Microsoft.Win32;
using Model.EditorData;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using Shell32;

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
            //fileSystemWatcher.Deleted += FileSystemWatcher_Changed;
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            fileSystemWatcher.Error += FileSystemWatcher_Error;
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

        [STAThread]
        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            wasDeleted = true;
            deletedPath = e.FullPath;


            //ControllerProject controller = new ControllerProject();
            //if (isInRecycleBin(deletedPath))
            //    wasDeleted = false; // because folders don't fire the 'changed' event when being moved to the Recycle Bin.

            //controller.OnDeleteElement(deletedPath);

            /* I explicitly write this comment and put nothing in this 'else if' clause because:
                 * When deleting a directory from the filesystem it never actually fires the 'changed'
                 * event because the directory is actually 'moved' to the recycle bin.
                 * But it does fire when we delete a file.
                 * 
                 * EDIT: Apparently it DOES fire when recycle bin is empty?
                 * I'm not sure about this yet, but it seems to work now
                 */
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
                ControllerProject controller = new ControllerProject();
                controller.OnDeleteElement(deletedPath);

                InvokeDelete(sender, e);
            }
            else if (wasCreated)
            {
                ControllerProject controller = new ControllerProject();
                controller.OnCreateElement(createdPath, false);

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

        }

        /*
        [STAThread]
        private bool isInRecycleBin(string fullPath)
        {
            Shell32.Folder recycleBin = GetShell32RecycleBin();

            bool isInRecycleBin = false;
            string[] fileAtt = new string[52];
            foreach (Shell32.FolderItem2 item in recycleBin.Items())
            {
                for(int i = 0; i < fileAtt.Length; i++)
                {
                    fileAtt[i] = recycleBin.GetDetailsOf(item, i);
                }
                string FileName = recycleBin.GetDetailsOf(item, 0);
                string Attributes = recycleBin.GetDetailsOf(item, 26);
                string Location = recycleBin.GetDetailsOf(item, 39);
                if (Path.GetExtension(FileName) == "") FileName += Path.GetExtension(item.Path);
                
                string FilePath = recycleBin.GetDetailsOf(item, 1);
                if (fullPath == Path.Combine(FilePath, FileName))
                {
                    isInRecycleBin = true;
                }
            }


            return isInRecycleBin;

            /*
            for (int i = 0; i < Shl.Items().Count; i++)
            {
                FolderItem2 FI = Shl.Items().Item(i);
                string FileName = Shl.GetDetailsOf(FI, 0);
                if (Path.GetExtension(FileName) == "") FileName += Path.GetExtension(FI.Path);
                //Necessary for systems with hidden file extensions.
                string FilePath = Shl.GetDetailsOf(FI, 1);
                if (fullPath == Path.Combine(FilePath, FileName))
                {
                    return true;
                }
            }
            return false;
            */
        //}
    
        /*
        private Shell32.Folder GetShell32RecycleBin()
        {
            Type shellAppType = Type.GetTypeFromProgID("Shell.Application");
            Object shell = Activator.CreateInstance(shellAppType);
            return (Shell32.Folder)shellAppType.InvokeMember("NameSpace",
            System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { 0x000a }); // 0x000a == Recycle bin
            // Replace 0x000a with string folder path to get any folder
        }
        */
    }
}
