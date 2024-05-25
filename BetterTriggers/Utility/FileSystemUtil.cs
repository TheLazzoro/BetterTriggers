using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using War3Net.Build.Info;

namespace BetterTriggers.Utility
{
    public static class FileSystemUtil
    {
        public static void Move(string elementToMove, string targetDir, int insertIndex)
        {
            Project.CurrentProject.insertIndex = insertIndex;
            string directory = targetDir;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(targetDir);

            if (File.Exists(elementToMove))
            {
                string file = Path.Combine(directory, Path.GetFileName(elementToMove));
                if (File.Exists(file))
                    throw new Exception($"File '{file}' already exists!");
                File.Move(elementToMove, file);
            }
            else if (Directory.Exists(elementToMove))
            {
                string folder = Path.Combine(directory, Path.GetFileName(elementToMove));
                if (elementToMove == folder)
                    return;
                if (Directory.Exists(folder))
                    throw new Exception($"Directory '{folder}' already exists!");

                Directory.Move(elementToMove, folder);
            }
        }

        public static void Delete(string path)
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
        public static void Rename(string oldPath, string renamedText)
        {
            string newPath = Path.Combine(Path.GetDirectoryName(oldPath), renamedText);

            if (newPath == oldPath)
                return;

            if (File.Exists(newPath))
                throw new Exception($"File with name '{renamedText}' already exists in the directory.");
            if (Directory.Exists(newPath))
                throw new Exception($"Folder with name '{renamedText}' already exists in the directory.");

            if (File.Exists(oldPath))
                File.Move(oldPath, newPath);
            else if (Directory.Exists(oldPath))
                Directory.Move(oldPath, newPath);
        }

        /// <summary>
        /// Used to undo/redo renames.
        /// </summary>
        /// <param name="oldFullPath"></param>
        /// <param name="newFullPath"></param>
        public static void RenameElementPath(string oldFullPath, string newFullPath)
        {
            if (File.Exists(oldFullPath))
                File.Move(oldFullPath, newFullPath);
            else if (Directory.Exists(oldFullPath))
                Directory.Move(oldFullPath, newFullPath);
        }

        public static void OpenInExplorer(string fullPath, bool doSelect = true)
        {
            if (File.Exists(fullPath) || Directory.Exists(fullPath))
            {
                if (doSelect)
                {
                    Process.Start("explorer.exe", "/select, " + fullPath);
                }
                else
                {
                    Process.Start("explorer.exe", fullPath);
                }
            }
        }

        public static bool IsExtensionValid(string extension)
        {
            if (
                extension == "" ||
                extension == ".var" ||
                extension == ".trg" ||
                extension == ".j" ||
                extension == ".lua"
                )
                return true;

            return false;
        }

        /// <summary>
        /// Returns appropriate file extension.
        /// </summary>
        public static string GetExtension(ExplorerElement element, ScriptLanguage language)
        {
            string extension = string.Empty;
            switch (element.ElementType)
            {
                case ExplorerElementEnum.Script:
                    extension = language == ScriptLanguage.Jass ? ".j" : ".lua";
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    extension = ".var";
                    break;
                case ExplorerElementEnum.Trigger:
                    extension = ".trg";
                    break;
            }

            return extension;
        }

        public static string FormatFileOrDirectoryName(string entry)
        {
            entry = entry
                .Replace("<", "")
                .Replace(">", "")
                .Replace("\"", "")
                .Replace("|", "")
                .Replace("?", "")
                .Replace("*", "");

            return entry;
        }
    }
}
