using BetterTriggers.Containers;
using Microsoft.VisualBasic.FileIO;
using Model.EditorData;
using System;
using System.Collections.Generic;
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

        public void MoveFile(string elementToMove, string target, int insertIndex)
        {
            ContainerProject.insertIndex = insertIndex;
            string directory = target;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(target);

            if (File.Exists(elementToMove))
                File.Move(elementToMove, directory + "/" + Path.GetFileName(elementToMove));
            else if (Directory.Exists(elementToMove))
                Directory.Move(elementToMove, directory + "/" + Path.GetFileName(elementToMove));
        }

        public void DeleteElement(string path)
        {
            if (File.Exists(path))
                FileSystem.DeleteFile(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            else if (Directory.Exists(path))
                FileSystem.DeleteDirectory(path, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
        }

        // used when renaming an element in the editor
        public void RenameElement(string oldPath, string renamed)
        {
            string newPath = Path.GetDirectoryName(oldPath) + @"/" + renamed;
            File.Move(oldPath, newPath);
        }
    }
}
