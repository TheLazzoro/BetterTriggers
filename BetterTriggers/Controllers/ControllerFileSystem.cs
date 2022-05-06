using BetterTriggers.Containers;
using Microsoft.VisualBasic.FileIO;
using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BetterTriggers.Controllers
{
    public class ControllerFileSystem
    {
        public void SaveFile(string path, string json)
        {
            File.WriteAllText(path, json);
        }

        public void MoveFile(string elementToMove, string targetDir, int insertIndex)
        {
            ContainerProject.insertIndex = insertIndex;
            string directory = targetDir;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(targetDir);

            if (File.Exists(elementToMove))
                File.Move(elementToMove, directory + "/" + Path.GetFileName(elementToMove));
            else if (Directory.Exists(elementToMove))
            {
                var name = Path.GetFileName(elementToMove);
                if (elementToMove == directory + "\\" + name)
                    return;

                Directory.Move(elementToMove, directory + "/" + name);
            }
        }

        public void DeleteElement(string path)
        {
            if (File.Exists(path))
                //File.Delete(path);
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else if (Directory.Exists(path))
                //Directory.Delete(path);
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        /// <summary>
        /// Used when renaming an element in the editor.
        /// </summary>
        /// <param name="oldPath"></param>
        /// <param name="renamedText"></param>
        public void RenameElement(string oldPath, string renamedText)
        {
            string newPath = Path.GetDirectoryName(oldPath) + @"/" + renamedText;
            File.Move(oldPath, newPath);
        }

        /// <summary>
        /// Used to undo/redo renames.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        public void RenameElementPath(string oldFullPath, string newFullPath)
        {
            File.Move(oldFullPath, newFullPath);
        }

        public void OpenInExplorer(string fullPath)
        {
            if (Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = Path.GetDirectoryName(fullPath),
                    FileName = "explorer.exe"
                };

                Process.Start(startInfo);
            }
        }
    }
}
