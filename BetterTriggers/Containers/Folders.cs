using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Containers
{
    public class Folders
    {
        private HashSet<ExplorerElement> folderContainer = new HashSet<ExplorerElement>();

        /// <summary>
        /// Creates a folder at the current selected 'destination' folder.
        /// </summary>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Category";
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Directory.Exists(directory + @"\" + name))
                    ok = true;
                else
                {
                    name = "Untitled Category " + i;
                }

                i++;
            }

            string path = Path.Combine(directory, name);
            Directory.CreateDirectory(path);

            return path;
        }

        public void Clear()
        {
            folderContainer.Clear();
        }
        
        public void AddFolder(ExplorerElement folder)
        {
            folderContainer.Add(folder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in folderContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        public void Remove(ExplorerElement explorerElement)
        {
            folderContainer.Remove(explorerElement);
        }

        internal string GenerateName(string folder)
        {
            string name = folder;
            bool exists = true;
            int i = 0;
            while (exists)
            {
                if (Directory.Exists(name))
                {
                    name = folder + i;
                    i++;
                }
                else
                    exists = false;
            }

            return name;
        }
    }
}
