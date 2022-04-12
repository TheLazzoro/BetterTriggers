using BetterTriggers.Containers;
using Model.Data;
using Model.SaveableData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BetterTriggers.Controllers
{
    public class ControllerTrigger
    {
        public void CreateTrigger()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = "Untitled Trigger";

            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!ContainerTriggers.Contains(name))
                    ok = true;
                else
                {
                    name = "Untitled Trigger " + i;
                }

                i++;
            }

            Trigger trigger = new Trigger()
            {
                Id = ContainerTriggers.GenerateId(),
            };
            string json = JsonConvert.SerializeObject(trigger);

            File.WriteAllText(directory + @"\" + name + ".trg", json);
        }

        public Trigger LoadTriggerFromFile(string filename)
        {
            var file = File.ReadAllText(filename);
            Trigger trigger = JsonConvert.DeserializeObject<Trigger>(file);

            return trigger;
        }
    }
}
