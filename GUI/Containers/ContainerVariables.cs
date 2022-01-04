using GUI.Components.TriggerExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace GUI.Containers
{
    public static class ContainerVariables
    {
        private static List<ExplorerElement> triggerVariableContainer = new List<ExplorerElement>();

        public static void AddTriggerElement(ExplorerElement variable)
        {
            triggerVariableContainer.Add(variable);
        }

        public static int Count()
        {
            return triggerVariableContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public static bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerVariableContainer)
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
            return triggerVariableContainer[index];
        }

        public static void Remove(ExplorerElement explorerElement)
        {
            triggerVariableContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < triggerVariableContainer.Count; i++)
            {
                var item = triggerVariableContainer[i];
                if (item.FilePath == filePath)
                {
                    triggerVariableContainer.Remove(item);
                }
            }
        }
    }
}
