using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerFolders
    {
        private static List<ExplorerElement> folderContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement trigger)
        {
            folderContainer.Add(trigger);
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
                if (item.ElementName == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElement Get(int index)
        {
            return folderContainer[index];
        }

        public static void Remove(ExplorerElement explorerElement)
        {
            folderContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < folderContainer.Count; i++)
            {
                var item = folderContainer[i];
                if (item.FilePath == filePath)
                {
                    folderContainer.Remove(item);
                }
            }
        }
    }
}
