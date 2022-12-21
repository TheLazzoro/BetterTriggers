using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.WorldEdit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;

namespace BetterTriggers.Controllers
{
    public class ControllerScript
    {
        /// <returns>Full file path.</returns>
        public static string Create()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Script";
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Scripts.Contains(name))
                    ok = true;
                else
                {
                    name = "Untitled Script " + i;
                }

                i++;
            }

            string extension = ContainerProject.project.Language == "lua" ? ".lua" : ".j";
            string fullPath = Path.Combine(directory, name + extension);
            File.WriteAllText(fullPath, "");

            return fullPath;
        }

        public static string LoadFromFile(string filePath)
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
