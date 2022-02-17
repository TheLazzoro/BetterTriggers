using Facades.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;

namespace Facades.Controllers
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

            File.WriteAllText(directory + @"\" + name + ".j", "");
        }

        public string LoadScriptFromFile(string filePath)
        {
            string script = string.Empty;
            if (File.Exists(filePath))
                script = File.ReadAllText(filePath);

            return script;
        }

        /*
        public ScriptControl CreateScriptControlWithScript(TabControl tabControl, string filePath)
        {
            var scriptControl = CreateScriptControl(tabControl);
            scriptControl.textEditor.Text = LoadScriptFromFile(filePath);

            return scriptControl;
        }

        private ScriptControl CreateScriptControl(TabControl tabControl)
        {
            var scriptControl = new ScriptControl();

            // Position editor
            Grid.SetColumn(scriptControl, 1);
            Grid.SetRow(scriptControl, 2);
            Grid.SetRowSpan(scriptControl, 3);

            return scriptControl;
        }
        */
    }
}
