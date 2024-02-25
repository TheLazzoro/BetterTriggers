using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BetterTriggers.Utility;
using Newtonsoft.Json;
using System.IO;
using BetterTriggers.Commands;
using BetterTriggers.WorldEdit;
using System.Xml.Linq;
using System.Collections.ObjectModel;

namespace BetterTriggers.Containers
{
    public class Triggers
    {
        public Trigger SelectedTrigger { get; set; }
        private HashSet<ExplorerElement> triggerElementContainer = new HashSet<ExplorerElement>();
        private ExplorerElement lastCreated;

        public void AddTrigger(ExplorerElement trigger)
        {
            triggerElementContainer.Add(trigger);
            lastCreated = trigger;
        }

        public int GenerateId()
        {
            int generatedId = 0;
            bool isIdValid = false;
            while (!isIdValid)
            {
                bool doesIdExist = false;
                var enumerator = triggerElementContainer.GetEnumerator();
                while (!doesIdExist && enumerator.MoveNext())
                {
                    if (enumerator.Current.trigger.Id == generatedId)
                        doesIdExist = true;
                }

                if (!doesIdExist)
                    isIdValid = true;
                else
                    generatedId = RandomUtil.GenerateInt();
            }

            return generatedId;
        }

        public int Count()
        {
            return triggerElementContainer.Count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Returns true if an element with the given file name exists in the container.</returns>
        public bool Contains(string name)
        {
            bool found = false;

            foreach (var item in triggerElementContainer)
            {
                if (item.GetName().ToLower() == name.ToLower()) // ToLower because filesystem is case-insensitive
                {
                    found = true;
                }
            }

            return found;
        }

        public bool Contains(int id)
        {
            bool found = false;
            foreach (var item in triggerElementContainer)
            {
                if (item.GetId() == id)
                {
                    found = true;
                }
            }
            return found;
        }

        public string GetName(int triggerId)
        {
            var element = GetById(triggerId);
            return element.GetName();
        }

        public ExplorerElement GetById(int id)
        {
            ExplorerElement trigger = null;
            var enumerator = triggerElementContainer.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.trigger.Id == id)
                {
                    trigger = enumerator.Current;
                    break;
                }
            }

            return trigger;
        }

        public ExplorerElement GetLastCreated()
        {
            return lastCreated;
        }

        public List<ExplorerElement> GetAll()
        {
            return triggerElementContainer.Select(x => x).ToList();
        }

        public void Remove(ExplorerElement explorerElement)
        {
            triggerElementContainer.Remove(explorerElement);
        }

        public ExplorerElement GetByReference(TriggerRef triggerRef)
        {
            return GetById(triggerRef.TriggerId);
        }

        /// <returns>Full file path.</returns>
        public string Create()
        {
            string directory = Project.CurrentProject.currentSelectedElement;
            if (!Directory.Exists(directory))
                directory = Path.GetDirectoryName(directory);

            string name = GenerateTriggerName();

            Trigger_Saveable trigger = new Trigger_Saveable()
            {
                Id = GenerateId(),
            };
            string json = JsonConvert.SerializeObject(trigger);

            string fullPath = Path.Combine(directory, name);
            File.WriteAllText(fullPath, json);

            return fullPath;
        }

        internal string GenerateTriggerName(string name = "Untitled Trigger")
        {
            string generatedName = name;
            bool ok = false;
            int i = 0;
            while (!ok)
            {
                if (!Contains(generatedName))
                    ok = true;
                else
                {
                    generatedName = name + i;
                }

                i++;
            }

            return generatedName + ".trg";
        }


        /// <summary>
        /// Creates a list of saveable trigger refs
        /// </summary>
        public List<TriggerRef> GetTriggerRefs()
        {
            List<ExplorerElement> elements = GetAll();
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



        /// <returns>A list of all parameters given to a TriggerElement</returns>
        public static List<Parameter> GetElementParametersAll(TriggerElement te)
        {
            ECA eca = (ECA)te;
            var list = GetElementParametersAll(eca.function.parameters);
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

        public void CopyTriggerElements(ExplorerElement copiedFrom, TriggerElementCollection copiedCollection, bool isCut = false)
        {
            var type = copiedCollection.Elements[0].ElementType;
            TriggerElementCollection copiedItems = new TriggerElementCollection(type);
            for (int i = 0; i < copiedCollection.Count(); i++)
            {
                if (copiedCollection.Elements[i] is ECA)
                {
                    var element = (ECA)copiedCollection.Elements[i];
                    copiedItems.Elements.Add(element.Clone());
                }
                else if (copiedCollection.Elements[i] is LocalVariable)
                {
                    var element = (LocalVariable)copiedCollection.Elements[i];
                    copiedItems.Elements.Add(element.Clone());
                }
            }

            CopiedElements.CopiedTriggerElements = copiedItems;

            if (isCut)
            {
                CopiedElements.CutTriggerElements = copiedCollection;
                CopiedElements.CopiedFromTrigger = copiedFrom;
            }
            else
                CopiedElements.CutTriggerElements = null;
        }

        /// <returns>A list of pasted elements.</returns>
        public TriggerElementCollection PasteTriggerElements(ExplorerElement destinationTrigger, TriggerElementCollection parentList, int insertIndex)
        {
            var copied = CopiedElements.CopiedTriggerElements;
            var pasted = new TriggerElementCollection(copied.ElementType);
            for (int i = 0; i < copied.Count(); i++)
            {
                if (copied.Elements[i] is ECA eca)
                {
                    pasted.Elements.Add(eca.Clone());
                }
                else if (copied.Elements[i] is LocalVariable localVar)
                {
                    var clone = localVar.Clone();
                    var variables = Project.CurrentProject.Variables;
                    clone.variable.Id = variables.GenerateId();
                    clone.variable.Name = variables.GenerateLocalName(destinationTrigger.trigger, clone.variable.Name);
                    pasted.Elements.Add(clone);
                    variables.AddLocalVariable(clone);
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
        public bool RemoveInvalidReferences(ExplorerElement explorerElement)
        {
            int removeCount = 0;
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Events);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Conditions);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.LocalVariables);
            removeCount += RemoveInvalidReferences(explorerElement.trigger, explorerElement.trigger.Actions);

            return removeCount > 0;
        }

        public int RemoveInvalidReferences(Trigger trig, TriggerElementCollection triggerElements)
        {
            int removeCount = 0;

            for (int i = 0; i < triggerElements.Elements.Count; i++)
            {
                if (triggerElements.Elements[i] is LocalVariable localVar)
                {
                    if (localVar.variable.InitialValue is Value value)
                    {
                        bool dataExists = CustomMapData.ReferencedDataExists(value, localVar.variable.Type);
                        if (!dataExists)
                        {
                            localVar.variable.InitialValue = new Parameter();
                            removeCount += 1;
                        }
                    }
                    continue;
                }

                var eca = (ECA)triggerElements.Elements[i];
                bool ecaExists = TriggerData.FunctionExists(eca.function);
                if (!ecaExists)
                {
                    triggerElements.Elements[i] = new InvalidECA();
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
        private int VerifyParametersAndRemove(Trigger trig, List<Parameter> parameters, List<string> returnTypes)
        {
            int removeCount = 0;

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];


                if (parameter is VariableRef varRef)
                {
                    Variable variable = Project.CurrentProject.Variables.GetById(varRef.VariableId, trig);
                    if (variable == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is TriggerRef)
                {
                    var trigger = GetByReference(parameter as TriggerRef);
                    if (trigger == null || trigger.trigger == null)
                    {
                        removeCount++;
                        parameters[i] = new Parameter();
                    }
                }
                else if (parameter is Value value)
                {
                    bool refExists = CustomMapData.ReferencedDataExists(value, returnTypes[i]);
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
                else if (parameter is Preset preset)
                {
                    bool constantExists = TriggerData.ConstantExists(preset.value);
                    if (!constantExists)
                    {
                        parameters[i] = new Parameter();
                        removeCount++;
                    }
                }
            }

            return removeCount;
        }

        // TODO: This seems incomplete and intended functionality probably exists in another method.
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
                    var variable = Project.CurrentProject.Variables.GetVariableById_AllLocals(varRef.VariableId);
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

        public int VerifyParametersInTrigger(ExplorerElement explorerTrigger)
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
            var triggers = GetAll();
            List<Function> functions = new List<Function>();
            triggers.ForEach(trig => functions.AddRange(GetFunctionsFromTrigger(trig)));

            return functions;
        }

        public static List<Function> GetFunctionsFromTrigger(ExplorerElement explorerElement)
        {
            List<Function> list = new List<Function>();
            list.AddRange(GatherFunctions(explorerElement.trigger.Events));
            list.AddRange(GatherFunctions(explorerElement.trigger.Conditions));
            list.AddRange(GatherFunctions(explorerElement.trigger.Actions));

            return list;
        }

        private static List<Function> GatherFunctions(TriggerElementCollection triggerElements)
        {
            List<Function> list = new List<Function>();
            triggerElements.Elements.ForEach(t =>
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
                    Variable variable = Project.CurrentProject.Variables.GetById(variableRef.VariableId);
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
            var triggers = GetAll();
            List<Parameter> parameters = new List<Parameter>();
            triggers.ForEach(trig => parameters.AddRange(GetParametersFromTrigger(trig)));

            return parameters;
        }

        public List<VariableRef> GetVariableRefsFromTrigger(ExplorerElement explorerElement)
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
        public static List<Parameter> GetParametersFromTrigger(ExplorerElement explorerElement)
        {
            List<Parameter> list = new List<Parameter>();
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Events));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Conditions));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.LocalVariables));
            list.AddRange(GatherTriggerParameters(explorerElement.trigger.Actions));

            return list;
        }

        private static List<Parameter> GatherTriggerParameters(TriggerElementCollection triggerElements)
        {
            List<Parameter> parameters = new List<Parameter>();

            for (int i = 0; i < triggerElements.Elements.Count; i++)
            {
                var triggerElement = triggerElements.Elements[i];
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
