using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Containers
{
    public static class Scripts
    {
        private static List<ExplorerElementScript> scriptContainer = new List<ExplorerElementScript>();

        public static void AddScript(ExplorerElementScript script)
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
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        public static ExplorerElementScript Get(int index)
        {
            return scriptContainer[index];
        }

        public static void Remove(ExplorerElementScript explorerElement)
        {
            scriptContainer.Remove(explorerElement);
        }

        public static void RemoveByFilePath(string filePath)
        {
            for (int i = 0; i < scriptContainer.Count; i++)
            {
                var item = scriptContainer[i];
                if (item.GetPath() == filePath)
                {
                    scriptContainer.Remove(item);
                }
            }
        }

        internal static void Clear()
        {
            scriptContainer.Clear();
        }
    }
}
