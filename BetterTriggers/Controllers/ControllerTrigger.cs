using BetterTriggers.Commands;
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

            string name = GenerateTriggerName();

            Trigger trigger = new Trigger()
            {
                Id = ContainerTriggers.GenerateId(),
            };
            string json = JsonConvert.SerializeObject(trigger);

            File.WriteAllText(directory + @"\" + name, json);
        }

        public string GenerateTriggerName()
        {
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

            return name + ".trg";
        }

        public Trigger LoadTriggerFromFile(string filename)
        {
            var file = File.ReadAllText(filename);
            Trigger trigger = JsonConvert.DeserializeObject<Trigger>(file);

            return trigger;
        }

        public void CopyTriggerElements(List<TriggerElement> list, bool isCut = false)
        {
            List<TriggerElement> copiedItems = new List<TriggerElement>();
            for (int i = 0; i < list.Count; i++)
            {
                copiedItems.Add((TriggerElement)list[i].Clone());
            }
            ContainerCopiedElements.CopiedTriggerElements = copiedItems;

            if (isCut)
                ContainerCopiedElements.CutTriggerElements = list;
            else
                ContainerCopiedElements.CutTriggerElements = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentList"></param>
        /// <param name="insertIndex"></param>
        /// <returns>A list of pasted elements.</returns>
        public List<TriggerElement> PasteTriggerElements(List<TriggerElement> parentList, int insertIndex)
        {
            var copied = ContainerCopiedElements.CopiedTriggerElements;
            var pasted = new List<TriggerElement>();
            for (int i = 0; i < copied.Count; i++)
            {
                pasted.Add((TriggerElement)copied[i].Clone());
            }

            if (ContainerCopiedElements.CutTriggerElements == null)
            {
                CommandTriggerElementPaste command = new CommandTriggerElementPaste(pasted, parentList, insertIndex);
                command.Execute();
            }
            else
            {
                CommandTriggerElementCutPaste command = new CommandTriggerElementCutPaste(pasted, parentList, insertIndex);
                command.Execute();

            }

            return pasted;
        }
    }
}
