using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using War3Net.Build;
using War3Net.Build.Script;

namespace BetterTriggers
{
    internal class BT2WE
    {
        private Project _project;
        private Map _map;

        public BT2WE(Map map)
        {
            _map = map;
        }

        /// <summary>
        /// Converts all project files from Better Triggers to triggers used by the World Editor.
        /// </summary>
        internal void Convert()
        {
            _project = Project.CurrentProject;
            var triggers = _project.Triggers.GetAll();
            var variables = _project.Variables.GetAll();
            var actionDefinitions = _project.ActionDefinitions.GetAll();
            var conditionDefinitions = _project.ConditionDefinitions.GetAll();
            var functionDefinitions = _project.FunctionDefinitions.GetAll();

            int localVarCount = triggers.Where(t => t.GetLocalVariables().Count() > 0).Count();
            var varCustomFeatures = variables.Where(v => v.IsTwoDimensions).ToList();

            if (
                localVarCount > 0
                || varCustomFeatures.Count > 0
                || actionDefinitions.Count > 0
                || conditionDefinitions.Count > 0
                || functionDefinitions.Count > 0
                )
            {
                throw new Exception("Map contains custom Better Triggers data. Conversion is not possible.");
            }

            _map.CustomTextTriggers.CustomTextTriggers.Clear();
            RecurseThroughTriggers(_project.GetRoot(), -1);
            var mapTriggers = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
            mapTriggers.GameVersion = 2;
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.RootCategory, countRoot);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.UNK1, 0);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.Category, countCategory);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.Gui, countGui);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.Comment, 0);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.Script, countScript);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.Variable, countVariable);
            mapTriggers.TriggerItemCounts.Add(TriggerItemType.UNK7, 0);
            mapTriggers.TriggerItems.AddRange(triggerItems);
            mapTriggers.Variables.AddRange(variableDefinitions);
            _map.Triggers = mapTriggers;
        }

        List<TriggerItem> triggerItems = new List<TriggerItem>();
        List<VariableDefinition> variableDefinitions = new List<VariableDefinition>();
        int countRoot = 0;
        int countCategory = 0;
        int countGui = 0;
        int countScript = 0;
        int countVariable = 0;
        private void RecurseThroughTriggers(ExplorerElement explorerElement, int parentId)
        {
            int id = 0;
            switch (explorerElement.ElementType)
            {
                case ExplorerElementEnum.Folder:
                    countCategory++;
                    id = _project.GenerateId();
                    var triggerCategory = new TriggerCategoryDefinition();
                    triggerCategory.Id = id;
                    triggerCategory.ParentId = parentId;
                    triggerCategory.Name = explorerElement.GetName();
                    break;
                case ExplorerElementEnum.GlobalVariable:
                    countVariable++;
                    id = explorerElement.GetId();
                    var variable = explorerElement.variable;
                    var variableDefiniton = new TriggerVariableDefinition(TriggerItemType.Variable);
                    variableDefiniton.ParentId = parentId;
                    variableDefiniton.Id = id;
                    variableDefiniton.Name = explorerElement.GetName();
                    triggerItems.Add(variableDefiniton);

                    string initialValue = string.Empty;
                    if (!string.IsNullOrEmpty(variable.InitialValue.value))
                    {
                        initialValue = variable.InitialValue.value;
                    }
                    var variableDefinition2 = new VariableDefinition();
                    variableDefinition2.Id = id;
                    variableDefinition2.ParentId = parentId;
                    variableDefinition2.Name = explorerElement.GetName();
                    variableDefinition2.InitialValue = initialValue;
                    variableDefinition2.Type = variable.War3Type.Type;
                    variableDefinition2.IsArray = variable.IsArray;
                    variableDefinition2.ArraySize = variable.ArraySize[0];
                    variableDefinitions.Add(variableDefinition2);

                    break;
                case ExplorerElementEnum.Root:
                    countRoot++;
                    var root = new TriggerCategoryDefinition(TriggerItemType.RootCategory);
                    id = 0;
                    root.Id = id;
                    root.ParentId = -1;
                    root.Name = _project.MapName;
                    _map.CustomTextTriggers.GlobalCustomScriptComment = _project.war3project.Comment;
                    _map.CustomTextTriggers.GlobalCustomScriptCode.Code = _project.war3project.Header;
                    triggerItems.Add(root);
                    break;
                case ExplorerElementEnum.Script:
                    countScript++;
                    id = _project.GenerateId();
                    var script = new TriggerDefinition(TriggerItemType.Script);
                    script.Id = id;
                    script.ParentId = parentId;
                    script.Name = explorerElement.GetName();
                    script.IsEnabled = explorerElement.IsEnabled;
                    script.IsInitiallyOn = true;
                    script.IsCustomTextTrigger = true;
                    var customTextTrigger = new CustomTextTrigger();
                    customTextTrigger.Code = explorerElement.script;
                    _map.CustomTextTriggers.CustomTextTriggers.Add(customTextTrigger);
                    triggerItems.Add(script);
                    break;
                case ExplorerElementEnum.Trigger:
                    countGui++;
                    id = explorerElement.GetId();
                    var trigger = explorerElement.trigger;
                    var triggerDefinition = new TriggerDefinition(TriggerItemType.Gui);
                    triggerDefinition.Id = id;
                    triggerDefinition.Name = explorerElement.GetName();
                    triggerDefinition.Description = explorerElement.trigger.Comment;
                    triggerDefinition.ParentId = parentId;
                    triggerDefinition.RunOnMapInit = trigger.RunOnMapInit;
                    triggerDefinition.IsEnabled = explorerElement.IsEnabled;
                    triggerDefinition.IsInitiallyOn = explorerElement.IsInitiallyOn;
                    triggerDefinition.IsCustomTextTrigger = explorerElement.trigger.IsScript;
                    string customTriggerTextCode = string.Empty;
                    if (triggerDefinition.IsCustomTextTrigger)
                    {
                        customTriggerTextCode = explorerElement.script;
                    }
                    else
                    {
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Events));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Conditions));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Actions));
                    }

                    var customTextTrigger2 = new CustomTextTrigger();
                    customTextTrigger2.Code = customTriggerTextCode;
                    _map.CustomTextTriggers.CustomTextTriggers.Add(customTextTrigger2);
                    triggerItems.Add(triggerDefinition);
                    break;
            }

            if (explorerElement.ElementType == ExplorerElementEnum.Folder || explorerElement.ElementType == ExplorerElementEnum.Root)
            {
                var children = explorerElement.GetExplorerElements();
                for (int i = 0; i < children.Count; i++)
                {
                    var child = children[i];
                    RecurseThroughTriggers(child, id);
                }
            }
        }

        /// <param name="triggerElementCollection">The collection/branch attached to a trigger element</param>
        /// <param name="branch">In an if-then-else block, if=0 then=1 else=2</param>
        /// <returns></returns>
        private List<TriggerFunction> ConvertTriggerElements(TriggerElementCollection triggerElementCollection, int? branch = null)
        {
            var triggerFunctions = new List<TriggerFunction>();

            for (int i = 0; i < triggerElementCollection.Count(); i++)
            {
                var eca = triggerElementCollection.Elements[i] as ECA;
                List<string> returnTypes = BetterTriggers.WorldEdit.TriggerData.GetParameterReturnTypes(eca.function, null);
                TriggerFunction triggerFunction = new TriggerFunction();
                triggerFunction.Name = eca.function.value;
                triggerFunction.IsEnabled = eca.IsEnabled;
                triggerFunction.Branch = branch;
                switch (eca.ElementType)
                {
                    case TriggerElementType.Event:
                        triggerFunction.Type = TriggerFunctionType.Event;
                        break;
                    case TriggerElementType.Condition:
                        triggerFunction.Type = TriggerFunctionType.Condition;
                        break;
                    case TriggerElementType.Action:
                        triggerFunction.Type = TriggerFunctionType.Action;
                        break;
                    default:
                        break;
                }
                triggerFunction.Parameters.AddRange(ConvertTriggerFunctionParameters(eca.function.parameters, returnTypes));
                triggerFunctions.Add(triggerFunction);

                if (eca.Elements != null && eca.Elements.Count > 0)
                {
                    for (int c = 0; c < eca.Elements.Count; c++)
                    {
                        var collection = eca.Elements[c];
                        ConvertTriggerElements(collection as TriggerElementCollection, c);
                    }
                }
            }

            return triggerFunctions;
        }

        /// TODO: This method does not implement a solution for TRIGSTR parameters,
        /// and thus strings we be inlined in the WTG as opposed to listed in the WTS.
        /// Compare with <see cref="TriggerConverter"/>
        /// Needs testing.
        private List<TriggerFunctionParameter> ConvertTriggerFunctionParameters(List<Parameter> parameters, List<string> returnTypes)
        {
            var functionParameters = new List<TriggerFunctionParameter>();

            for (int i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                var returnType = returnTypes[i];
                string paramValue = parameter.value;
                if (paramValue == "boolexpr" && parameter is Function)
                {
                    paramValue = string.Empty;
                }

                var converted = new TriggerFunctionParameter();
                converted.Value = paramValue;
                functionParameters.Add(converted);

                switch (parameter)
                {
                    case Function function:
                        converted.Type = TriggerFunctionParameterType.Function;
                        converted.Value = paramValue;
                        if (function.parameters.Count > 0)
                        {
                            converted.Function = new TriggerFunction();
                            converted.Function.Name = paramValue;
                            converted.Function.Type = TriggerFunctionType.Call;
                            if(WorldEdit.TriggerData.ConditionTemplates.TryGetValue(paramValue, out var temp))
                            {
                                converted.Function.Type = TriggerFunctionType.Condition;
                            }
                            converted.Function.IsEnabled = true;
                        }
                        if (
                            paramValue == "ForGroup"
                            || paramValue == "ForLoopA"
                            || paramValue == "ForLoopB"
                            || paramValue == "ForLoopVar"
                            || paramValue == "EnumDestructablesInRectAll"
                            || paramValue == "EnumDestructablesInCircleBJ"
                            || paramValue == "EnumItemsInRectBJ"
                            || paramValue == "GetUnitsInRectAll"
                            || paramValue == "ForForce"
                            ) // special case for single-action loops
                        {
                            converted.Value = "DoNothing";
                            converted.Function.Name = paramValue;
                        }

                        var returnTypes1 = BetterTriggers.WorldEdit.TriggerData.GetParameterReturnTypes(function, null);
                        if (converted.Function != null)
                        {
                            converted.Function.Parameters.AddRange(ConvertTriggerFunctionParameters(function.parameters, returnTypes1));
                        }

                        break;
                    case Preset:
                        converted.Type = TriggerFunctionParameterType.Preset;
                        converted.Value = paramValue;
                        break;
                    case VariableRef varRef:
                        converted.Type = TriggerFunctionParameterType.Variable;
                        var variable = _project.Variables.GetByReference(varRef);
                        if (variable.IsArray)
                        {
                            // We only include the first dimension of the array. Two dimensions is an BT-only feature;
                            var arrayIndexParameter = new List<Parameter>()
                            {
                                varRef.arrayIndexValues[0]
                            };
                            var returnTypesArrayIndex = new List<string>();
                            returnTypesArrayIndex.Add("integer");
                            converted.ArrayIndexer = ConvertTriggerFunctionParameters(arrayIndexParameter, returnTypesArrayIndex).FirstOrDefault();
                        }
                        converted.Value = variable.Name;
                        break;
                    case TriggerRef triggerRef:

                        converted.Type = TriggerFunctionParameterType.Variable;
                        var element = _project.Triggers.GetById(triggerRef.TriggerId);
                        // TODO: Verify if we need the ASCII replace method here
                        paramValue = Ascii.ReplaceNonASCII(element.GetName().Replace(" ", "_"));
                        break;
                    case Value value:
                        converted.Type = TriggerFunctionParameterType.String;
                        string prefix = string.Empty;
                        if (returnType == "unit")
                            prefix = "gg_unit_";
                        else if (returnType == "item")
                            prefix = "gg_item_";
                        else if (returnType == "destructable")
                            prefix = "gg_dest_";
                        else if (returnType == "rect")
                            prefix = "gg_rct_";
                        else if (returnType == "camerasetup")
                            prefix = "gg_cam_";
                        else if (returnType == "sound")
                            prefix = "gg_snd_";

                        paramValue = Ascii.ReplaceNonASCII(paramValue.Replace(" ", "_")).Insert(0, prefix);

                        break;
                    default:
                        break;
                }

            }

            return functionParameters;
        }
    }
}
