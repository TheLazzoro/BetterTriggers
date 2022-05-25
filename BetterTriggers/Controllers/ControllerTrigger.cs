using BetterTriggers.Commands;
using BetterTriggers.Containers;
using Model.Data;
using Model.EditorData;
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

        /// <summary>
        /// </summary>
        /// <param name="function"></param>
        /// <returns>A list of all parameters given to a TriggerElement</returns>
        public List<Parameter> GetElementParametersAll(TriggerElement te)
        {
            List<Parameter> list = GetElementParametersAll(te.function.parameters);
            return list;
        }

        private List<Parameter> GetElementParametersAll(List<Parameter> parameters)
        {
            List<Parameter> list = new List<Parameter>();

            for (int i = 0; i < parameters.Count; i++)
            {
                list.Add(parameters[i]);
                if (parameters[i] is Function)
                {
                    Function f = (Function)parameters[i];
                    list.AddRange(GetElementParametersAll(f.parameters));
                }
            }

            return list;
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

        public bool RemoveInvalidReferences(ExplorerElementTrigger explorerElement)
        {
            int removeCount = 0;
            removeCount += RemoveInvalidReferences(explorerElement.trigger.Events);
            removeCount += RemoveInvalidReferences(explorerElement.trigger.Conditions);
            removeCount += RemoveInvalidReferences(explorerElement.trigger.Actions);

            return removeCount > 0;
        }

        private int RemoveInvalidReferences(List<TriggerElement> triggerElements)
        {
            int removeCount = 0;

            for (int i = 0; i < triggerElements.Count; i++)
            {
                var triggerElement = triggerElements[i];
                removeCount += VerifyParametersAndRemove(triggerElement.function.parameters);


                if(triggerElement.function is IfThenElse)
                {
                    var special = (IfThenElse)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.If);
                    removeCount += RemoveInvalidReferences(special.Then);
                    removeCount += RemoveInvalidReferences(special.Else);
                }
                else if(triggerElement.function is AndMultiple)
                {
                    var special = (AndMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.And);
                }
                else if (triggerElement.function is ForForceMultiple)
                {
                    var special = (ForForceMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is OrMultiple)
                {
                    var special = (OrMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Or);
                }
                else if (triggerElement.function is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement.function is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)triggerElement.function;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
            }

            return removeCount;
        }

        private int VerifyParametersAndRemove(List<Parameter> parameters)
        {
            int removeCount = 0;
            ControllerMapData controllerMapData = new ControllerMapData();

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if(parameter is VariableRef)
                {
                    ControllerVariable controllerVariable = new ControllerVariable();
                    Variable variable = controllerVariable.GetByReference(parameter as VariableRef);
                    if(variable == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter() { returnType = parameter.returnType };
                    }
                }
                else if(parameter is Value)
                {
                    bool exists = controllerMapData.ReferencedDataExists(parameter as Value);
                    if(!exists)
                    {
                        removeCount++;
                        parameters[i] = new Parameter() { returnType = parameter.returnType };
                    }

                }

                if(parameter is Function)
                {
                    var function = (Function)parameter;
                    removeCount += VerifyParametersAndRemove(function.parameters);
                }
            }

            return removeCount;
        }

        public int VerifyParameters(List<Parameter> parameters)
        {
            int invalidCount = 0;
            ControllerMapData controllerMapData = new ControllerMapData();

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if(parameter.identifier == null && parameter.returnType != "nothing")
                    invalidCount++;

                if(parameter is Function)
                {
                    var function = (Function)parameter;
                    invalidCount += VerifyParameters(function.parameters);
                }
            }

            return invalidCount;
        }


        /// <summary>
        /// </summary>
        /// <returns>A list of every parameter in every trigger.</returns>
        public List<Parameter> GetParametersAll()
        {
            var triggers = ContainerTriggers.GetAll();
            List<Parameter> parameters = new List<Parameter>();
            triggers.ForEach(trig => parameters.AddRange(GatherTriggerElements(trig)));

            return parameters;
        }

        private List<Parameter> GatherTriggerElements(ExplorerElementTrigger explorerElement)
        {
            List<Parameter> list = new List<Parameter>();
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Events));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Conditions));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Actions));

            return list;
        }

        private List<Parameter> GatherTriggerParameters(List<TriggerElement> triggerElements)
        {
            List<Parameter> parameters = new List<Parameter>();
            ControllerTrigger controller = new ControllerTrigger();

            for (int i = 0; i < triggerElements.Count; i++)
            {
                var triggerElement = triggerElements[i];
                parameters.AddRange(controller.GetElementParametersAll(triggerElement));


                if (triggerElement.function is IfThenElse)
                {
                    var special = (IfThenElse)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.If));
                    parameters.AddRange(GatherTriggerParameters(special.Then));
                    parameters.AddRange(GatherTriggerParameters(special.Else));
                }
                else if (triggerElement.function is AndMultiple)
                {
                    var special = (AndMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.And));
                }
                else if (triggerElement.function is ForForceMultiple)
                {
                    var special = (ForForceMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is OrMultiple)
                {
                    var special = (OrMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Or));
                }
                else if (triggerElement.function is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement.function is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)triggerElement.function;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
            }

            return parameters;
        }
    }
}
