using Model.War3Project;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Controls;

namespace GUI.Controllers
{
    public class ControllerProject
    {
        public void CreateProject(string language, string name, string destinationFolder)
        {
            War3Project project = new War3Project()
            {
                Language = language,
                Header = "",
                Root = destinationFolder + "//" + name,
                Files = new List<string>(),
            };

            string json = JsonConvert.SerializeObject(project);

            string filepath = destinationFolder + "//" + name + ".json";
            File.WriteAllText(filepath, json);
            Directory.CreateDirectory(destinationFolder + "//" + name);
        }

        public void LoadProject(TriggerExplorer triggerExplorer, Grid mainGrid, string filepath)
        {
            string json = File.ReadAllText(filepath);
            War3Project project = JsonConvert.DeserializeObject<War3Project>(json);
            string rootFolder = project.Root;

            ControllerTrigger controllerTrigger = new ControllerTrigger();

            string[] entries = Directory.GetFileSystemEntries(rootFolder);
            foreach (var entry in entries)
            {
                if (Directory.Exists(entry))
                    throw new NotImplementedException();
                else if(File.Exists(entry))
                {
                    var file = File.ReadAllText(entry);
                    Model.Trigger trigger = JsonConvert.DeserializeObject<Model.Trigger>(file);

                    controllerTrigger.CreateTriggerWithElements(triggerExplorer, mainGrid, entry, trigger);
                }
            }
        }
    }
}
