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
        private Dictionary<ExplorerElement, int> _folderIds = new Dictionary<ExplorerElement, int>();

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
            int varCustomFeaturesCount = variables.Where(v => v.IsTwoDimensions).Count();

            if (
                localVarCount > 0
                || varCustomFeaturesCount > 0
                || actionDefinitions.Count > 0
                || conditionDefinitions.Count > 0
                || functionDefinitions.Count > 0
                )
            {
                throw new Exception("Map contains custom Better Triggers data. Conversion is not possible.");
            }

            var triggerItems = RecurseThroughTriggers(_project.GetRoot());
            var mapTriggers = new MapTriggers(MapTriggersFormatVersion.v7, MapTriggersSubVersion.v4);
            mapTriggers.TriggerItems.AddRange(triggerItems);
            _map.Triggers = mapTriggers;
        }

        private List<TriggerItem> RecurseThroughTriggers(ExplorerElement parent)
        {
            var triggerItems = new List<TriggerItem>();
            _folderIds.TryGetValue(parent, out int parentId);
            var children = parent.GetExplorerElements();
            for (int i = 0; i < children.Count; i++)
            {
                var explorerElement = children[i];
                switch (explorerElement.ElementType)
                {
                    case ExplorerElementEnum.Folder:
                        int id = _project.GenerateId();
                        var triggerCategory = new TriggerCategoryDefinition();
                        triggerCategory.Id = id;
                        triggerCategory.ParentId = parentId;
                        triggerCategory.Name = explorerElement.GetName();
                        _folderIds.Add(explorerElement, id);
                        triggerItems.AddRange(RecurseThroughTriggers(explorerElement));
                        break;
                    case ExplorerElementEnum.GlobalVariable:
                        var variable = explorerElement.variable;
                        var variableDefiniton = new TriggerDefinition(TriggerItemType.Variable);
                        variableDefiniton.ParentId = parentId;
                        variableDefiniton.Id = explorerElement.GetId();
                        variableDefiniton.Name = explorerElement.GetName();
                        triggerItems.Add(variableDefiniton);
                        break;
                    case ExplorerElementEnum.Root:
                        var root = new TriggerCategoryDefinition(TriggerItemType.RootCategory);
                        root.Id = 0;
                        root.ParentId = -1;
                        root.Name = _project.MapName;
                        _map.CustomTextTriggers.GlobalCustomScriptComment = _project.war3project.Comment;
                        _map.CustomTextTriggers.GlobalCustomScriptCode.Code = _project.war3project.Header;
                        _folderIds.Add(explorerElement, root.Id);
                        break;
                    case ExplorerElementEnum.Script:
                        var script = new TriggerDefinition(TriggerItemType.Script);
                        script.Id = _project.GenerateId();
                        script.ParentId = parentId;
                        script.Name = explorerElement.GetName();
                        script.IsEnabled = explorerElement.IsEnabled;
                        script.IsInitiallyOn = true;
                        var customTextTrigger = new CustomTextTrigger();
                        customTextTrigger.Code = explorerElement.script;
                        _map.CustomTextTriggers.CustomTextTriggers.Add(customTextTrigger);
                        triggerItems.Add(script);
                        break;
                    case ExplorerElementEnum.Trigger:
                        var trigger = explorerElement.trigger;
                        var triggerDefinition = new TriggerDefinition(TriggerItemType.Gui);
                        triggerDefinition.Id = explorerElement.GetId();
                        triggerDefinition.Name = explorerElement.GetName();
                        triggerDefinition.Description = explorerElement.trigger.Comment;
                        triggerDefinition.ParentId = parentId;
                        triggerDefinition.RunOnMapInit = trigger.RunOnMapInit;
                        triggerDefinition.IsEnabled = explorerElement.IsEnabled;
                        triggerDefinition.IsInitiallyOn = explorerElement.IsInitiallyOn;
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Events));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Conditions));
                        triggerDefinition.Functions.AddRange(ConvertTriggerElements(trigger.Actions));
                        triggerItems.Add(triggerDefinition);
                        break;
                    default:
                        continue;
                }
            }

            return triggerItems;
        }

        /// <param name="triggerElementCollection">The collection/branch attached to a trigger element</param>
        /// <param name="branch">In an if-then-else block, if=0 then=1 else=2</param>
        /// <returns></returns>
        private List<TriggerFunction> ConvertTriggerElements(TriggerElementCollection triggerElementCollection, int branch = 0)
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
                        converted.Function = new TriggerFunction();
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
                        converted.Function.Parameters.AddRange(ConvertTriggerFunctionParameters(function.parameters, returnTypes1));

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
