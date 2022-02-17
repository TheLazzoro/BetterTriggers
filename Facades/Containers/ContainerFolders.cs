using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace Facades.Containers
{
    public static class ContainerFolders
    {
        private static List<ExplorerElementFolder> folderContainer = new List<ExplorerElementFolder>();

        public static void Clear()
        {
            folderContainer.Clear();
        }
        
        public static void AddFolder(ExplorerElementFolder folder)
        {
            folderContainer.Add(folder);
        }

        public static int Count()
        {
            return folderContainer.Count;
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
                if (item.GetName() == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElementFolder Get(int index)
        {
            return folderContainer[index];
        }

        public static void Remove(ExplorerElementFolder explorerElement)
        {
            folderContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < folderContainer.Count; i++)
            {
                var item = folderContainer[i];
                if (item.GetPath() == filePath)
                {
                    folderContainer.Remove(item);
                }
            }
        }
    }
}
