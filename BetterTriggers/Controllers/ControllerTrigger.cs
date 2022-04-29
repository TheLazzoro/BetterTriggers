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

        public string GetParameterDisplayName(Parameter parameter)
        {
            string name = "PLACEHOLDER";

            if (parameter is Function)
            {
                var function = (Function)parameter;
                if (function.parameters.Count > 0)
                    name = parameter.identifier;
                else
                {
                    ControllerTriggerData controller = new ControllerTriggerData();
                    name = controller.GetParamDisplayName(function);
                }
            }
            else if (parameter is Constant)
            {
                var constant = (Constant)parameter;
                ControllerTriggerData controller = new ControllerTriggerData();
                name = controller.GetParamDisplayName(constant);
            }
            else if (parameter is VariableRef)
            {

                // TODO: This will crash if a referenced variable is deleted.
                var variableRef = (VariableRef)parameter;
                var variable = ContainerVariables.GetVariableById(variableRef.VariableId);
                name = ContainerVariables.GetVariableNameById(variable.Id);

                // This exists in case a variable has been changed
                // TODO: WE NEED TO IMPLEMENT THIS IN A DIFFERENT WAY
                /*
                if (name == null || name == "" || variable.Type != parameter.returnType)
                {
                    parameters[paramIndex] = new Parameter()
                    {
                        returnType = variableRef.returnType,
                    };
                    varName = "null";
                }
                */
            } else if(parameter is Value)
            {
                // TODO: This will crash if a referenced variable is deleted.
                var value = (Value)parameter;
                name = value.identifier;

                // This exists in case a variable has been changed
                // TODO: NEED TO IMPLEMENT THIS IN A DIFFERENT WAY
                /*
                if (name == null || name == "" || value.returnType != parameters[paramIndex].returnType)
                {
                    parameters[paramIndex] = new Parameter()
                    {
                        returnType = value.returnType,
                    };
                    name = "null";
                }
                */
            }

            return name;

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
