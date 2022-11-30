using BetterTriggers.Commands;
using BetterTriggers.Containers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BetterTriggers.WorldEdit;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.EditorData;
using System.Linq;

namespace BetterTriggers.Controllers
{
    public class ControllerTrigger
    {
        /// <returns>Full file path.</returns>
        public string CreateTrigger()
        {
            string directory = ContainerProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateTriggerName();

            Trigger trigger = new Trigger()
            {
                Id = Triggers.GenerateId(),
            };
            string json = JsonConvert.SerializeObject(trigger);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }



        /// <summary>
        /// Creates a list of saveable trigger refs
        /// </summary>
        public List<TriggerRef> GetTriggerRefs()
        {
            List<ExplorerElementTrigger> elements = GetTriggersAll();
            List<TriggerRef> list = new List<TriggerRef>();

            for (int i = 0; i < elements.Count; i++)
            {
                TriggerRef trigRef = new TriggerRef()
                {
                    TriggerId = elements[i].trigger.Id,
                };

                list.Add(trigRef);
            }

            return list;
        }

        public string GenerateTriggerName()
        {
            string name = "Untitled Trigger";
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Triggers.Contains(name))
                    ok = true;
                else
                {
                    name = "Untitled Trigger " + i;
                }

                i++;
            }

            return name + ".trg";
        }

        public string GetTriggerName(int triggerId)
        {
            return Triggers.GetName(triggerId);
        }

        public Trigger GetById(int id)
        {
            return Triggers.FindById(id).trigger;
        }



        public List<ExplorerElementTrigger> GetTriggersAll()
        {
            return Triggers.GetAll();
        }

        /// <returns>A list of all parameters given to a TriggerElement</returns>
        public List<Parameter> GetElementParametersAll(TriggerElement te)
        {
            ECA eca = (ECA)te;
            List<Parameter> list = GetElementParametersAll(eca.function.parameters);
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

        public void CopyTriggerElements(ExplorerElementTrigger copiedFrom, List<TriggerElement> list, bool isCut = false)
        {
            List<TriggerElement> copiedItems = new List<TriggerElement>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is ECA)
                {
                    var element = (ECA)list[i];
                    copiedItems.Add(element.Clone());
                }
                else if (list[i] is LocalVariable)
                {
                    var element = (LocalVariable)list[i];
                    copiedItems.Add(element.Clone());
                }
            }
            CopiedElements.CopiedTriggerElements = copiedItems;

            if (isCut)
            {
                CopiedElements.CutTriggerElements = list;
                CopiedElements.CopiedFromTrigger = copiedFrom;
            }
            else
                CopiedElements.CutTriggerElements = null;
        }

        /// <returns>A list of pasted elements.</returns>
        public List<TriggerElement> PasteTriggerElements(ExplorerElementTrigger destinationTrigger, List<TriggerElement> parentList, int insertIndex)
        {
            var copied = CopiedElements.CopiedTriggerElements;
            var pasted = new List<TriggerElement>();
            for (int i = 0; i < copied.Count; i++)
            {
                if (copied[i] is ECA)
                {
                    var element = (ECA)copied[i];
                    pasted.Add(element.Clone());
                }
                else if (copied[i] is LocalVariable)
                {
                    var element = (LocalVariable)copied[i];
                    var clone = element.Clone();
                    clone.variable.Id = Variables.GenerateId();
                    clone.variable.Name = Variables.GenerateLocalName(destinationTrigger.trigger, clone.variable.Name);
                    pasted.Add(clone);
                    Variables.AddLocalVariable(clone);
                }
            }

            if (CopiedElements.CutTriggerElements == null)
            {
                CommandTriggerElementPaste command = new CommandTriggerElementPaste(destinationTrigger, pasted, parentList, insertIndex);
                command.Execute();
            }
            else
            {
                CommandTriggerElementCutPaste command = new CommandTriggerElementCutPaste(CopiedElements.CopiedFromTrigger, destinationTrigger, pasted, parentList, insertIndex);
                command.Execute();
            }

            return pasted;
        }

        /// <returns>Whether the trigger had invalid references removed.</returns>
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
                var triggerElement = (ECA)triggerElements[i];
                List<string> returnTypes = TriggerData.GetParameterReturnTypes(triggerElement.function);
                removeCount += VerifyParametersAndRemove(triggerElement.function.parameters, returnTypes);


                if (triggerElement is IfThenElse)
                {
                    var special = (IfThenElse)triggerElement;
                    removeCount += RemoveInvalidReferences(special.If);
                    removeCount += RemoveInvalidReferences(special.Then);
                    removeCount += RemoveInvalidReferences(special.Else);
                }
                else if (triggerElement is AndMultiple)
                {
                    var special = (AndMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.And);
                }
                else if (triggerElement is ForForceMultiple)
                {
                    var special = (ForForceMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is OrMultiple)
                {
                    var special = (OrMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Or);
                }
                else if (triggerElement is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
                else if (triggerElement is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)triggerElement;
                    removeCount += RemoveInvalidReferences(special.Actions);
                }
            }

            return removeCount;
        }

        private int VerifyParametersAndRemove(List<Parameter> parameters, List<string> returnTypes)
        {
            int removeCount = 0;
            ControllerMapData controllerMapData = new ControllerMapData();

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter is VariableRef)
                {
                    ControllerVariable controllerVariable = new ControllerVariable();
                    Variable variable = controllerVariable.GetByReference(parameter as VariableRef);
                    if (variable == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is TriggerRef)
                {
                    ControllerTrigger controllerTrig = new ControllerTrigger();
                    Trigger trigger = controllerTrig.GetByReference(parameter as TriggerRef);
                    if (trigger == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is Value)
                {
                    bool exists = controllerMapData.ReferencedDataExists(parameter as Value, returnTypes[i]);
                    if (!exists)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }

                }

                if (parameter is Function)
                {
                    var function = (Function)parameter;
                    List<string> _returnTypes = TriggerData.GetParameterReturnTypes(function);
                    removeCount += VerifyParametersAndRemove(function.parameters, _returnTypes);
                }
            }

            return removeCount;
        }

        public Trigger GetByReference(TriggerRef triggerRef)
        {
            var explorerElement = Triggers.GetByReference(triggerRef);
            if (explorerElement == null)
                return null;

            Trigger trig = explorerElement.trigger;
            return trig;
        }

        /// <summary>
        /// Returns amount of invalid parameters.
        /// </summary>
        public int VerifyParameters(List<Parameter> parameters)
        {
            int invalidCount = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter.value == null && !(parameter is VariableRef) && !(parameter is TriggerRef))
                    invalidCount++;

                if (parameter is Function)
                {
                    var function = (Function)parameter;
                    invalidCount += VerifyParameters(function.parameters);
                }
                else if (parameter is VariableRef varRef)
                {
                    ControllerVariable controller = new ControllerVariable();
                    var variable = controller.GetByReference(varRef);
                    if (variable == null)
                        invalidCount++;
                    else
                    {
                        List<Parameter> arrays = new List<Parameter>();
                        if (variable.IsArray)
                            arrays.Add(varRef.arrayIndexValues[0]);
                        if (variable.IsArray && variable.IsTwoDimensions)
                            arrays.Add(varRef.arrayIndexValues[1]);
                        
                        invalidCount += VerifyParameters(arrays);
                    }
                }
            }

            return invalidCount;
        }

        public int VerifyParametersInTrigger(ExplorerElementTrigger explorerTrigger)
        {
            List<Parameter> list = GetParametersFromTrigger(explorerTrigger);
            int invalidCount = VerifyParameters(list);
            return invalidCount;
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of every function in every trigger. This also includes inner functions in parameters.</returns>
        public List<Function> GetFunctionsAll()
        {
            var triggers = Triggers.GetAll();
            List<Function> functions = new List<Function>();
            triggers.ForEach(trig => functions.AddRange(GetFunctionsFromTrigger(trig)));

            return functions;
        }

        public List<Function> GetFunctionsFromTrigger(ExplorerElementTrigger explorerElement)
        {
            List<Function> list = new List<Function>();
            list.AddRange(GatherFunctions(explorerElement.trigger.Events));
            list.AddRange(GatherFunctions(explorerElement.trigger.Conditions));
            list.AddRange(GatherFunctions(explorerElement.trigger.Actions));

            return list;
        }

        private List<Function> GatherFunctions(List<TriggerElement> triggerElements)
        {
            List<Function> list = new List<Function>();
            triggerElements.ForEach(t =>
            {
                var eca = (ECA)t;
                list.AddRange(GetFunctionsFromParameters(eca.function));

                if (t is IfThenElse)
                {
                    var special = (IfThenElse)t;
                    list.AddRange(GatherFunctions(special.If));
                    list.AddRange(GatherFunctions(special.Then));
                    list.AddRange(GatherFunctions(special.Else));
                }
                else if (t is AndMultiple)
                {
                    var special = (AndMultiple)t;
                    list.AddRange(GatherFunctions(special.And));
                }
                else if (t is ForForceMultiple)
                {
                    var special = (ForForceMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is OrMultiple)
                {
                    var special = (OrMultiple)t;
                    list.AddRange(GatherFunctions(special.Or));
                }
                else if (t is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
                else if (t is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)t;
                    list.AddRange(GatherFunctions(special.Actions));
                }
            });

            return list;
        }

        private List<Function> GetFunctionsFromParameters(Function function)
        {
            List<Function> list = new List<Function>();
            list.Add(function);
            function.parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    ControllerVariable controller = new ControllerVariable();
                    VariableRef variableRef = p as VariableRef;
                    Variable variable = controller.GetByReference(variableRef);
                    if (variable == null)
                        return;

                    if (variable.IsArray)
                    {
                        if (variableRef.arrayIndexValues[0] is Function)
                            list.AddRange(GetFunctionsFromParameters(variableRef.arrayIndexValues[0] as Function));
                    }
                    if (variable.IsTwoDimensions)
                    {
                        if (variableRef.arrayIndexValues[1] is Function)
                            list.AddRange(GetFunctionsFromParameters(variableRef.arrayIndexValues[1] as Function));
                    }
                }

                if (p is Function)
                    list.AddRange(GetFunctionsFromParameters(p as Function));
            });

            return list;
        }


        /// <summary>
        /// </summary>
        /// <returns>A list of every parameter in every trigger.</returns>
        public List<Parameter> GetParametersAll()
        {
            var triggers = Triggers.GetAll();
            List<Parameter> parameters = new List<Parameter>();
            triggers.ForEach(trig => parameters.AddRange(GetParametersFromTrigger(trig)));

            return parameters;
        }

        public List<VariableRef> GetVariableRefsFromTrigger(ExplorerElementTrigger explorerElement)
        {
            List<Parameter> _params = GetParametersFromTrigger(explorerElement);
            List<VariableRef> variableRefs = new List<VariableRef>();
            _params.ForEach(p =>
            {
                var varRef = p as VariableRef;
                if (varRef != null)
                    variableRefs.Add(varRef);
            });

            return variableRefs;
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of every parameter in the given trigger.</returns>
        public List<Parameter> GetParametersFromTrigger(ExplorerElementTrigger explorerElement)
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


                if (triggerElement is IfThenElse)
                {
                    var special = (IfThenElse)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.If));
                    parameters.AddRange(GatherTriggerParameters(special.Then));
                    parameters.AddRange(GatherTriggerParameters(special.Else));
                }
                else if (triggerElement is AndMultiple)
                {
                    var special = (AndMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.And));
                }
                else if (triggerElement is ForForceMultiple)
                {
                    var special = (ForForceMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is OrMultiple)
                {
                    var special = (OrMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Or));
                }
                else if (triggerElement is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
                else if (triggerElement is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)triggerElement;
                    parameters.AddRange(GatherTriggerParameters(special.Actions));
                }
            }

            return parameters;
        }

        public string GetValueName(string key, string returnType)
        {
            string text = key;
            switch (returnType)
            {
                case "unit":
                    text = $"{UnitTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "item":
                    text = $"{ItemTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "destructable":
                    text = $"{DestructibleTypes.GetName(key.Substring(0, 4))} {key.Substring(5, key.Length - 5)} <gen>";
                    break;
                case "camerasetup":
                    text = $"{key} <gen>";
                    break;
                case "rect":
                    text = $"{key} <gen>";
                    break;
                case "unitcode":
                    text = UnitTypes.GetName(key);
                    break;
                case "destructablecode":
                    text = DestructibleTypes.GetName(key);
                    break;
                case "abilcode":
                    text = AbilityTypes.GetName(key);
                    break;
                case "buffcode":
                    text = BuffTypes.GetName(key);
                    break;
                case "techcode":
                    text = UpgradeTypes.GetName(key);
                    break;
                case "itemcode":
                    text = ItemTypes.GetName(key);
                    break;
                case "doodadcode":
                    text = DoodadTypes.GetName(key);
                    break;
                default:
                    break;
            }

            if (text == null)
                text = string.Empty;

            return text;
        }

        public string GetFourCCDisplay(string key, string returnType)
        {
            string text = string.Empty;
            if (returnType == "unitcode")
                text = $"[{key}] ";
            else if (returnType == "destructablecode")
                text = $"[{key}] ";
            else if (returnType == "abilcode")
                text = $"[{key}] ";
            else if (returnType == "buffcode")
                text = $"[{key}] ";
            else if (returnType == "techcode")
                text = $"[{key}] ";
            else if (returnType == "itemcode")
                text = $"[{key}] ";
            else if (returnType == "doodadcode")
                text = $"[{key}] ";

            return text;
        }
    }
}
