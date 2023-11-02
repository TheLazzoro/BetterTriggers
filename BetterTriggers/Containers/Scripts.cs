using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace BetterTriggers.Containers
{
    public static class Scripts
    {
        private static HashSet<ExplorerElementScript> scriptContainer = new HashSet<ExplorerElementScript>();

        public static void AddScript(ExplorerElementScript script)
        {
            scriptContainer.Add(script);
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

        public static void Remove(ExplorerElementScript explorerElement)
        {
            scriptContainer.Remove(explorerElement);
        }

        internal static void Clear()
        {
            scriptContainer.Clear();
        }

        internal static string GenerateName(ExplorerElementScript script)
        {
            string path = script.GetPath();
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string extension = Project.project.Language == "lua" ? ".lua" : ".j";
            int i = 0;
            bool exists = true;
            while (exists)
            {
                if (File.Exists(path))
                {
                    path = Path.Combine(folder, filename + i + extension);
                    i++;
                }
                else
                    exists = false;
            }

            return path;
        }
    }
}
