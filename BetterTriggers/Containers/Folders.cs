using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class Folders
    {
        private static HashSet<ExplorerElementFolder> folderContainer = new HashSet<ExplorerElementFolder>();

        public static void Clear()
        {
            folderContainer.Clear();
        }
        
        public static void AddFolder(ExplorerElementFolder folder)
        {
            folderContainer.Add(folder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
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

        public static void Remove(ExplorerElementFolder explorerElement)
        {
            folderContainer.Remove(explorerElement);
        }

    }
}
