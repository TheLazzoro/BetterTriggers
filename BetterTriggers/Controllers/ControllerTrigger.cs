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
        public static Trigger SelectedTrigger { get; set; }


        /// <returns>Full file path.</returns>
        public static string Create()
        {
            string directory = Project.currentSelectedElement;
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
        public static List<TriggerRef> GetTriggerRefs()
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

        public static string GenerateTriggerName(string name = "Untitled Trigger")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Triggers.Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }

            return generatedName + ".trg";
        }

        public static string GetTriggerName(int triggerId)
        {
            return Triggers.GetName(triggerId);
        }

        public static Trigger GetById(int id)
        {
            return Triggers.FindById(id).trigger;
        }

        public static List<ExplorerElementTrigger> GetTriggersAll()
        {
            return Triggers.GetAll();
        }

        /// <returns>A list of all parameters given to a TriggerElement</returns>
        public static List<Parameter> GetElementParametersAll(TriggerElement te)
        {
            ECA eca = (ECA)te;
            List<Parameter> list = GetElementParametersAll(eca.function.parameters);
            return list;
        }

        private static List<Parameter> GetElementParametersAll(List<Parameter> parameters)
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

        public static void CopyTriggerElements(ExplorerElementTrigger copiedFrom, List<TriggerElement> list, bool isCut = false)
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
        public static List<TriggerElement> PasteTriggerElements(ExplorerElementTrigger destinationTrigger, List<TriggerElement> parentList, int insertIndex)
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
        public static bool RemoveInvalidReferences(ExplorerElementTrigger explorerElement)
        {
            int removeCount = 0;
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Events);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Conditions);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.LocalVariables);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Actions);

            return removeCount > 0;
        }

        public static int RemoveInvalidReferences(Trigger trig, List<TriggerElement> triggerElements)
        {
            int removeCount = 0;

            for (int i = 0; i < triggerElements.Count; i++)
            {
                if (triggerElements[i] is LocalVariable localVar)
                {
                    if (localVar.variable.InitialValue is Value value)
                    {
                        bool dataExists = ControllerMapData.ReferencedDataExists(value, localVar.variable.Type);
                        if (!dataExists)
                        {
                            localVar.variable.InitialValue = new Parameter();
                            removeCount += 1;
                        }
                    }
                    continue;
                }

                var eca = (ECA)triggerElements[i];
                bool ecaExists = TriggerData.FunctionExists(eca.function);
                if(!ecaExists)
                {
                    triggerElements[i] = new InvalidECA();
                    removeCount += 1;
                }
                List<string> returnTypes = TriggerData.GetParameterReturnTypes(eca.function);
                removeCount += VerifyParametersAndRemove(trig, eca.function.parameters, returnTypes);


                if (eca is IfThenElse)
                {
                    var special = (IfThenElse)eca;
                    removeCount += RemoveInvalidReferences(trig, special.If);
                    removeCount += RemoveInvalidReferences(trig, special.Then);
                    removeCount += RemoveInvalidReferences(trig, special.Else);
                }
                else if (eca is AndMultiple)
                {
                    var special = (AndMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.And);
                }
                else if (eca is ForForceMultiple)
                {
                    var special = (ForForceMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is ForGroupMultiple)
                {
                    var special = (ForGroupMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is ForLoopAMultiple)
                {
                    var special = (ForLoopAMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is ForLoopBMultiple)
                {
                    var special = (ForLoopBMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is ForLoopVarMultiple)
                {
                    var special = (ForLoopVarMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is OrMultiple)
                {
                    var special = (OrMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Or);
                }
                else if (eca is EnumDestructablesInRectAllMultiple)
                {
                    var special = (EnumDestructablesInRectAllMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is EnumDestructiblesInCircleBJMultiple)
                {
                    var special = (EnumDestructiblesInCircleBJMultiple)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
                else if (eca is EnumItemsInRectBJ)
                {
                    var special = (EnumItemsInRectBJ)eca;
                    removeCount += RemoveInvalidReferences(trig, special.Actions);
                }
            }

            return removeCount;
        }

        /// <param name="trig">Trigger to be verified.</param>
        /// <param name="parameters"></param>
        /// <param name="returnTypes"></param>
        /// <returns></returns>
        private static int VerifyParametersAndRemove(Trigger trig, List<Parameter> parameters, List<string> returnTypes)
        {
            int removeCount = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                

                if (parameter is VariableRef varRef)
                {
                    Variable variable = ControllerVariable.GetByReference(varRef, trig);
                    if (variable == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is TriggerRef)
                {
                    Trigger trigger = GetByReference(parameter as TriggerRef);
                    if (trigger == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is Value value)
                {
                    bool refExists = ControllerMapData.ReferencedDataExists(value, returnTypes[i]);
                    if (!refExists)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }

                }

                if (parameter is Function function)
                {
                    bool functionExists = TriggerData.FunctionExists(function);
                    if (!functionExists)
                    {
                        parameters[i] = new Parameter();
                        removeCount++;
                    }

                    List<string> _returnTypes = TriggerData.GetParameterReturnTypes(function);
                    removeCount += VerifyParametersAndRemove(trig, function.parameters, _returnTypes);
                }
                else if(parameter is Constant constant)
                {
                    bool constantExists = TriggerData.ConstantExists(constant.value);
                    if (!constantExists)
                    {
                        parameters[i] = new Parameter();
                        removeCount++;
                    }
                }
            }

            return removeCount;
        }

        public static Trigger GetByReference(TriggerRef triggerRef)
        {
            var explorerElement = Triggers.GetByReference(triggerRef);
            if (explorerElement == null)
                return null;

            Trigger trig = explorerElement.trigger;
            return trig;
        }

        // TODO: This seems incomplete and intended functionality probably exists in another method.
        /// <summary>
        /// Returns amount of invalid parameters.
        /// </summary>
        public static int VerifyParameters(List<Parameter> parameters)
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
                    var variable = ControllerVariable.GetByReference_AllLocals(varRef);
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

        public static int VerifyParametersInTrigger(ExplorerElementTrigger explorerTrigger)
        {
            List<Parameter> list = GetParametersFromTrigger(explorerTrigger);
            int invalidCount = VerifyParameters(list);
            return invalidCount;
        }

        /// <summary>
        /// </summary>
        /// <returns>A list of every function in every trigger. This also includes inner functions in parameters.</returns>
        public static List<Function> GetFunctionsAll()
        {
            var triggers = Triggers.GetAll();
            List<Function> functions = new List<Function>();
            triggers.ForEach(trig => functions.AddRange(GetFunctionsFromTrigger(trig)));

            return functions;
        }

        public static List<Function> GetFunctionsFromTrigger(ExplorerElementTrigger explorerElement)
        {
            List<Function> list = new List<Function>();
            list.AddRange(GatherFunctions(explorerElement.trigger.Events));
            list.AddRange(GatherFunctions(explorerElement.trigger.Conditions));
            list.AddRange(GatherFunctions(explorerElement.trigger.Actions));

            return list;
        }

        private static List<Function> GatherFunctions(List<TriggerElement> triggerElements)
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

        private static List<Function> GetFunctionsFromParameters(Function function)
        {
            List<Function> list = new List<Function>();
            list.Add(function);
            function.parameters.ForEach(p =>
            {
                if (p is VariableRef)
                {
                    VariableRef variableRef = p as VariableRef;
                    Variable variable = ControllerVariable.GetByReference(variableRef);
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
        public static List<Parameter> GetParametersAll()
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
        public static List<Parameter> GetParametersFromTrigger(ExplorerElementTrigger explorerElement)
        {
            List<Parameter> list = new List<Parameter>();
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Events));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Conditions));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.LocalVariables));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Actions));

            return list;
        }

        private static List<Parameter> GatherTriggerParameters(List<TriggerElement> triggerElements)
        {
            List<Parameter> parameters = new List<Parameter>();

            for (int i = 0; i < triggerElements.Count; i++)
            {
                var triggerElement = triggerElements[i];
                if (triggerElement is LocalVariable localVar)
                {
                    parameters.Add(localVar.variable.InitialValue);
                    continue;
                }

                parameters.AddRange(GetElementParametersAll(triggerElement));

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

        public static string GetValueName(string key, string returnType)
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
                case "string":
                case "StringExt":
                    if (key == string.Empty)
                        text = "<Empty String>";
                    break;
                case "frameevents":
                    text = "Events...";
                    break;
                default:
                    break;
            }

            if (text == null)
                text = string.Empty;

            return text;
        }

        public static string GetFourCCDisplay(string key, string returnType)
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
