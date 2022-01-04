using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerTriggers
    {
        private static List<ExplorerElement> triggerElementContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement trigger)
        {
            triggerElementContainer.Add(trigger);
        }

        public static int Count()
        {
            return triggerElementContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerElementContainer)
            {
                if(item.ElementName == name)
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElement Get(int index)
        {
            return triggerElementContainer[index];
        }

        public static void Remove(ExplorerElement explorerElement)
        {
            triggerElementContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for( int i = 0; i < triggerElementContainer.Count; i++)
            {
                var item = triggerElementContainer[i];
                if (item.FilePath == filePath)
                {
                    triggerElementContainer.Remove(item);
                }
            }
        }
    }
}
