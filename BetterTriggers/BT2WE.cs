﻿using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.Templates;
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
            var explorerElementsWithBTOnlyFeatures = new List<Tuple<ExplorerElement, string>>(); // [explorer element, error reason]

            var triggers = _project.Triggers.GetAll();
            var variables = _project.Variables.GetGlobals();
            var elementsWithLocals = triggers.Where(t => t.GetLocalVariables().Count() > 0);
            var varCustomFeatures = variables.Where(v => v.variable.IsTwoDimensions).ToList();
            var actionDefinitions = _project.ActionDefinitions.GetAll();
            var conditionDefinitions = _project.ConditionDefinitions.GetAll();
            var functionDefinitions = _project.FunctionDefinitions.GetAll();

            foreach (var item in elementsWithLocals)
            {
                explorerElementsWithBTOnlyFeatures.Add(new(item, "Cannot convert local variables."));
            }
            foreach (var item in varCustomFeatures)
            {
                explorerElementsWithBTOnlyFeatures.Add(new(item, "Is a two-dimensional array."));
            }
            foreach (var item in actionDefinitions)
            {
                explorerElementsWithBTOnlyFeatures.Add(new(item, "Cannot convert action definitions."));
            }
            foreach (var item in conditionDefinitions)
            {
                explorerElementsWithBTOnlyFeatures.Add(new(item, "Cannot convert condition definitions."));
            }
            foreach (var item in functionDefinitions)
            {
                explorerElementsWithBTOnlyFeatures.Add(new(item, "Cannot convert function definitions."));
            }


            var allExplorerElements = _project.GetAllExplorerElements();
            for (int i = 0; i < allExplorerElements.Count; i++)
            {
                var explorerElement = allExplorerElements[i];
                if(explorerElement.ElementType == ExplorerElementEnum.Trigger && explorerElement.trigger.IsScript)
                {
                    continue;
                }
                var functions = Function.GetFunctionsFromTrigger(explorerElement);
                for (int j = 0; j < functions.Count; j++)
                {
                    var function = functions[j];
                    bool btOnlyData;
                    for (int k = 0; k < function.parameters.Count; k++)
                    {
                        var parameter = function.parameters[k];
                        if (parameter.value == null)
                        {
                            continue;
                        }

                        btOnlyData = WorldEdit.TriggerData.IsBTOnlyData(parameter.value);
                        if (btOnlyData)
                        {
                            string displayName = WorldEdit.TriggerData.GetParamDisplayName(parameter);
                            explorerElementsWithBTOnlyFeatures.Add(new(explorerElement, $"Uses Better Triggers-only parameter: '{displayName}' ({parameter.value})"));
                        }
                    }

                    btOnlyData = WorldEdit.TriggerData.IsBTOnlyData(function.value);
                    if (btOnlyData)
                    {
                        string displayName = WorldEdit.TriggerData.GetFuntionDisplayName(function.value);
                        explorerElementsWithBTOnlyFeatures.Add(new(explorerElement, $"Uses Better Triggers-only function: '{displayName}' ({function.value})"));
                    }
                }
            }

            if (explorerElementsWithBTOnlyFeatures.Count > 0)
            {
                throw new ContainsBTDataException(explorerElementsWithBTOnlyFeatures, "Map contains custom Better Triggers data. Conversion is not possible.");
            }

            if (_map.CustomTextTriggers == null)
            {
                _map.CustomTextTriggers = new MapCustomTextTriggers(MapCustomTextTriggersFormatVersion.v1, MapCustomTextTriggersSubVersion.v4);
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
                    triggerItems.Add(triggerCategory);
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
                    variableDefinition2.Unk = 1;
                    variableDefinitions.Add(variableDefinition2);

                    break;
                case ExplorerElementEnum.Root:
                    countRoot++;
                    var root = new TriggerCategoryDefinition(TriggerItemType.RootCategory);
                    id = 0;
                    root.Id = id;
                    root.ParentId = -1;
                    root.Name = _project.MapName + ".w3x";
                    if (_map.CustomTextTriggers.GlobalCustomScriptCode == null)
                    {
                        _map.CustomTextTriggers.GlobalCustomScriptCode = new CustomTextTrigger();
                    }
                    _map.CustomTextTriggers.GlobalCustomScriptComment = _project.war3project.Comment;
                    _map.CustomTextTriggers.GlobalCustomScriptCode.Code = _project.war3project.Header + "\0"; // Add NUL char
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
                    customTextTrigger.Code = explorerElement.script + "\0"; // Add NUL char
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
                        customTriggerTextCode = explorerElement.trigger.Script;
                    }
                    else
                    {
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Events));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Conditions));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Actions));
                    }

                    var customTextTrigger2 = new CustomTextTrigger();
                    customTextTrigger2.Code = customTriggerTextCode + "\0"; // Add NUL char
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
                        var childElements = ConvertTriggerElements(collection as TriggerElementCollection, c);
                        triggerFunction.ChildFunctions.AddRange(childElements);
                    }
                }
            }

            return triggerFunctions;
        }

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
                functionParameters.Add(converted);

                switch (parameter)
                {
                    case Function function:
                        converted.Type = TriggerFunctionParameterType.Function;
                        converted.Function = new TriggerFunction();
                        converted.Function.Name = paramValue;
                        converted.Function.Type = TriggerFunctionType.Call;
                        if (WorldEdit.TriggerData.EventTemplates.TryGetValue(paramValue, out var temp))
                        {
                            converted.Function.Type = TriggerFunctionType.Event;
                        }
                        else if (WorldEdit.TriggerData.ConditionTemplates.TryGetValue(paramValue, out var temp2))
                        {
                            converted.Function.Type = TriggerFunctionType.Condition;
                        }
                        else if (WorldEdit.TriggerData.ActionTemplates.TryGetValue(paramValue, out var temp3))
                        {
                            converted.Function.Type = TriggerFunctionType.Action;
                        }
                        converted.Function.IsEnabled = true;

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
                        if (function.parameters.Count > 0)
                        {
                            converted.Function.Parameters.AddRange(ConvertTriggerFunctionParameters(function.parameters, returnTypes1));
                        }

                        break;
                    case Preset:
                        converted.Type = TriggerFunctionParameterType.Preset;
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
                        paramValue = variable.Name;
                        break;
                    case TriggerRef triggerRef:

                        converted.Type = TriggerFunctionParameterType.Variable;
                        var element = _project.Triggers.GetById(triggerRef.TriggerId);
                        paramValue = "gg_trg_" + Ascii.ReplaceNonASCII(element.GetName().Replace(" ", "_"));
                        break;
                    case Value value:
                        converted.Type = TriggerFunctionParameterType.String;
                        string prefix = string.Empty;
                        bool isVariable = false;
                        if (returnType == "unit")
                        {
                            prefix = "gg_unit_";
                            isVariable = true;
                        }
                        else if (returnType == "item")
                        {
                            prefix = "gg_item_";
                            isVariable = true;
                        }
                        else if (returnType == "destructable")
                        {
                            prefix = "gg_dest_";
                            isVariable = true;
                        }
                        else if (returnType == "rect")
                        {
                            prefix = "gg_rct_";
                            isVariable = true;
                        }
                        else if (returnType == "camerasetup")
                        {
                            prefix = "gg_cam_";
                            isVariable = true;
                        }
                        else if (returnType == "sound")
                        {
                            prefix = "gg_snd_";
                            isVariable = true;
                        }
                        else if (returnType == "StringExt")
                        {
                            paramValue = AddTriggerString(paramValue);
                        }

                        if (isVariable)
                        {
                            converted.Type = TriggerFunctionParameterType.Variable;
                            paramValue = Ascii.ReplaceNonASCII(paramValue.Replace(" ", "_")).Insert(0, prefix);
                        }


                        break;
                    default:
                        break;
                }

                converted.Value = paramValue;
            }

            return functionParameters;
        }

        /// <summary>
        /// Returns the TRIGSTR_XXX reference.
        /// </summary>
        private string AddTriggerString(string stringToAdd)
        {
            int index = _map.TriggerStrings.Strings.Count - 2;
            var lastString = _map.TriggerStrings.Strings[index];

            var triggerString = new TriggerString();
            triggerString.Key = lastString.Key + 1;
            triggerString.Value = stringToAdd;
            triggerString.EmptyLineCount = 1;
            triggerString.KeyPrecision = (uint)Math.Floor(Math.Log10(triggerString.Key) + 1);
            _map.TriggerStrings.Strings.Insert(index + 1, triggerString);

            return $"TRIGSTR_{triggerString.Key}";
        }
    }
}
