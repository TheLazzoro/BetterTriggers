using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerScripts
    {
        private static List<ExplorerElement> scriptContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement script)
        {
            scriptContainer.Add(script);
        }

        public static int Count()
        {
            return scriptContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in scriptContainer)
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
            return scriptContainer[index];
        }

        public static void Remove(ExplorerElement explorerElement)
        {
            scriptContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < scriptContainer.Count; i++)
            {
                var item = scriptContainer[i];
                if (item.FilePath == filePath)
                {
                    scriptContainer.Remove(item);
                }
            }
        }
    }
}
