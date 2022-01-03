using GUI.Components.TriggerExplorer;
using GUI.Utility;
using Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerVariable
    {
        public void CreateVariable(Grid mainGrid, TriggerExplorer triggerExplorer)
        {
            ControllerProject controllerProject = new ControllerProject();
            string directory = controllerProject.GetDirectoryFromSelection(triggerExplorer.treeViewTriggerExplorer);
            string name = NameGenerator.GenerateVariableName();

            Variable trigger = new Variable();
            string json = JsonConvert.SerializeObject(trigger);

            File.WriteAllText(directory + "/" + name + ".json", json);
        }
    }
}
