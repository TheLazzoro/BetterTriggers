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
        public void CreateScript()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Script";

            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!ContainerScripts.Contains(name))
                    ok = true;
                else
                {
                    name = "Untitled Script " + i;
                }

                i++;
            }

            string extension = ContainerProject.project.Language == "lua" ? ".lua" : ".j";

            File.WriteAllText(directory + @"\" + name + extension, "");
        }

        public string LoadScriptFromFile(string filePath)
        {
            string script = string.Empty;
            if (File.Exists(filePath))
            {
                bool didLoad = false;
                while (!didLoad)
                {
                    try
                    {
                        script = File.ReadAllText(filePath);
                        didLoad = true;
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
                    } finally
                    {
                        Thread.Sleep(10);
                    }
                }
            }

            return script;
        }

        public ExplorerElementScript GetExplorerElementByFileName(string filePath)
        {
            ExplorerElementScript explorerElement = null;

            for (int i = 0; i < ContainerScripts.Count(); i++)
            {
                var element = ContainerScripts.Get(i);
                if (element.GetPath() == filePath)
                {
                    explorerElement = element;
                    break;
                }
            }

            return explorerElement;
        }
    }
}
