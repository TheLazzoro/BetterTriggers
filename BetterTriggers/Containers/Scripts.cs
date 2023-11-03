using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;
using System.Threading;

namespace BetterTriggers.Containers
{
    public class Scripts
    {
        private HashSet<ExplorerElementScript> scriptContainer = new HashSet<ExplorerElementScript>();

        public void AddScript(ExplorerElementScript script)
        {
            scriptContainer.Add(script);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
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

        public void Remove(ExplorerElementScript explorerElement)
        {
            scriptContainer.Remove(explorerElement);
        }

        internal void Clear()
        {
            scriptContainer.Clear();
        }

        internal string GenerateName(ExplorerElementScript script)
        {
            string path = script.GetPath();
            string folder = Path.GetDirectoryName(path);
            string filename = Path.GetFileNameWithoutExtension(path);
            string extension = Project.CurrentProject.war3project.Language == "lua" ? ".lua" : ".j";
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

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Script";
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Contains(name))
                    ok = true;
                else
                {
                    name = "Untitled Script " + i;
                }

                i++;
            }

            string extension = Project.CurrentProject.war3project.Language == "lua" ? ".lua" : ".j";
            string fullPath = Path.Combine(directory, name + extension);
            File.WriteAllText(fullPath, "");

            return fullPath;
        }


        public string LoadFromFile(string filePath)
        {
            string script = string.Empty;
            if (File.Exists(filePath))
            {
                bool didLoad = false;
                while (!didLoad)
                {
                    try
                    {
                        Thread.Sleep(10);
                        //script = File.ReadAllText(filePath);
                        //didLoad = true;
                        using (FileStream stream = new FileStream(filePath, FileMode.Open))
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                script = sr.ReadToEnd();
                                didLoad = true;
                            }
                        }
                        if (script == "") // TODO: obscure bug where script is empty on save
                        {

                        }
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine(e.StackTrace);
                    }
                }
            }

            return script;
        }
    }
}
