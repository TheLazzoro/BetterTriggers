using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Utility
{
    public class ParamTextBuilder
    {
        private List<HyperlinkBT> hyperlinkParameters = new List<HyperlinkBT>();
        private StringBuilder stringBuilder = new StringBuilder();
        private EditorSettings settings;
        private Project project;
        private Variable _variable;
        private ECA _eca;
        private ExplorerElement _explorerElement;

        public ParamTextBuilder()
        {
            settings = EditorSettings.Load();
            project = Project.CurrentProject;
        }

        public string GenerateTreeItemText(ExplorerElement explorerElement, ECA eca)
        {
            _explorerElement = explorerElement;
            _eca = eca;
            StringBuilder sb = new StringBuilder();
            List<Inline> Inlines = new List<Inline>();
            List<Inline> generated;
            var returnTypes = new List<string>();
            string paramText;
            if (eca is ActionDefinitionRef actionDefRef)
            {
                var actionDef = Project.CurrentProject.ActionDefinitions.FindById(actionDefRef.ActionDefinitionId).actionDefinition;
                actionDef.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    returnTypes.Add(paramDef.ReturnType.Type);
                });
                paramText = actionDef.ParamText;
                generated = RecurseGenerateParamText(paramText, eca.function.parameters, returnTypes);
            }
            else if (eca is ConditionDefinitionRef conditionDefRef)
            {
                var conditionDef = Project.CurrentProject.ConditionDefinitions.FindById(conditionDefRef.ConditionDefinitionId).conditionDefinition;
                conditionDef.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    returnTypes.Add(paramDef.ReturnType.Type);
                });
                paramText = TriggerData.GetParamText(eca.function);
                generated = RecurseGenerateParamText(paramText, eca.function.parameters, returnTypes);
            }
            else
            {
                returnTypes = TriggerData.GetParameterReturnTypes(eca.function);
                paramText = TriggerData.GetParamText(eca.function);
                generated = RecurseGenerateParamText(paramText, eca.function.parameters, returnTypes);
            }
            
            GenerateTreeItemText(sb, eca.function.parameters, returnTypes, paramText);

            return sb.ToString();
        }

        /// <summary>
        /// TODO: Probably fix copy-paste?
        /// 
        /// This is copy-pasted from 'RecurseGenerateParamText' and uses strings instead of UI components.
        /// It is slow to pull text out of 'Inline' UI components, so this serves as an optimized way
        /// to generate the header text for TreeViewItems when loading in large quantities.
        /// 
        /// Because of code duplication, remember to update both methods when something changes.
        /// 
        /// </summary>
        private void GenerateTreeItemText(StringBuilder sb, List<Parameter> parameters, List<string> returnTypes, string paramText)
        {
            int paramIndex = 0;
            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] != '~')
                {
                    sb.Append(paramText[i]);
                    continue;
                }

                if (sb.Length > 0 && sb[sb.Length - 1] == ',')
                    sb.Remove(sb.Length - 1, 1); // Removes comma before param

                if (parameters[paramIndex] is Function)
                {
                    var function = (Function)parameters[paramIndex];
                    if (function.parameters.Count > 0) // first bracket gets hyperlinked
                    {
                        List<string> _returnTypes = TriggerData.GetParameterReturnTypes(function);
                        sb.Append("(");
                        GenerateTreeItemText(sb, function.parameters, _returnTypes, TriggerData.GetParamText(function)); // recurse
                    }
                    else // whole displayname gets hyperlinked
                        sb.Append($"({TriggerData.GetParamDisplayName(function)}");

                    sb.Append(")");
                }
                else if (parameters[paramIndex] is Preset)
                    sb.Append(TriggerData.GetParamDisplayName(parameters[paramIndex]));

                else if (parameters[paramIndex] is VariableRef)
                {
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = project.Variables.GetByReference(variableRef, _explorerElement);
                    string varName = string.Empty;

                    string expectedType = null;
                    string actualType = null;
                    if (variable != null)
                    {
                        expectedType = Types.GetBaseType(returnTypes[paramIndex]);
                        actualType = Types.GetBaseType(variable.Type);
                    }

                    // This exists in case a variable has been changed
                    if (variable == null || !Types.AreTypesEqual(expectedType, actualType))
                        varName = "null";
                    else
                        varName = project.Variables.GetVariableNameById(variable.Id);

                    sb.Append(varName);

                    if (variable != null && variable.IsArray && !variable.IsTwoDimensions)
                    {
                        List<string> _returnTypes = new List<string>();
                        _returnTypes.Add("integer");
                        GenerateTreeItemText(sb, variableRef.arrayIndexValues, _returnTypes, "[,~Number,]");
                    }
                    else if (variable != null && variable.IsArray && variable.IsTwoDimensions)
                    {
                        List<string> _returnTypes = new List<string>();
                        _returnTypes.Add("integer");
                        _returnTypes.Add("integer");
                        GenerateTreeItemText(sb, variableRef.arrayIndexValues, _returnTypes, "[,~Number,][,~Number,]");
                    }
                }
                else if (parameters[paramIndex] is TriggerRef)
                {
                    var triggerRef = (TriggerRef)parameters[paramIndex];
                    var trigger = project.Triggers.GetByReference(triggerRef);
                    string triggerName = string.Empty;

                    // This exists in case a trigger name has been changed
                    if (trigger == null)
                        triggerName = "null";
                    else
                        triggerName = project.Triggers.GetName(trigger.trigger.Id);

                    sb.Append(triggerName);
                }
                else if (parameters[paramIndex] is ParameterDefinitionRef paramDefRef)
                {
                    var parameterDefs = _explorerElement.GetParameterCollection();
                    var paramDef = parameterDefs.GetByReference(paramDefRef);
                    string name;

                    if (paramDef == null)
                        name = "null";
                    else
                        name = paramDef.Name;

                    sb.Append(name);
                }
                else if (parameters[paramIndex] is Value)
                {
                    var value = (Value)parameters[paramIndex];
                    var name = project.Triggers.GetValueName(value.value, returnTypes[paramIndex]);

                    // This exists in case a variable has been changed
                    //if (name == null || name == "")
                    //{
                    //    parameters[paramIndex] = new Parameter();
                    //    name = "null";
                    //}
                    sb.Append(name);
                }
                else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set.
                {
                    i++; // avoids the '~' in the name
                    int startIndex = i; // store current letter index
                    int length = 0;
                    bool isParamNameSet = false;
                    while (!isParamNameSet && i < paramText.Length) // scan parameter display name
                    {
                        if (paramText[i] == ',')
                            isParamNameSet = true;
                        else
                        {
                            length++;
                            i++;
                        }
                    }
                    string paramName = paramText.Substring(startIndex, length);
                    sb.Append(paramName);
                }

                while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                {
                    i++;
                }
                paramIndex++;
            }
        }

        public List<Inline> GenerateParamText(Variable variable)
        {
            _variable = variable;
            var Inlines = new List<Inline>();
            var parameters = new List<Parameter>() { variable.InitialValue };
            var returnTypes = new List<string>() { variable.Type };
            var generated = RecurseGenerateParamText("~Value", parameters, returnTypes);

            // First and last inline must be a string.
            // Otherwise hyperlinks get cut from the treeitem header (WPF black magic).
            Inlines.Add(new Run(""));
            Inlines.AddRange(generated);
            Inlines.Add(new Run(""));

            return Inlines;
        }

        public List<Inline> GenerateParamText(ExplorerElement explorerElement, ECA eca)
        {
            _explorerElement = explorerElement;
            _eca = eca;
            List<Inline> Inlines = new List<Inline>();
            List<Inline> generated;
            var returnTypes = new List<string>();
            if (eca is ActionDefinitionRef actionDefRef)
            {
                var actionDef = Project.CurrentProject.ActionDefinitions.FindById(actionDefRef.ActionDefinitionId).actionDefinition;
                string paramText = actionDef.ParamText;
                actionDef.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    string type = paramDef.ReturnType.Type;
                    returnTypes.Add(type);
                });

                generated = RecurseGenerateParamText(paramText, actionDefRef.function.parameters, returnTypes);
            }
            else if (eca is ConditionDefinitionRef conditionDefRef)
            {
                var conditionDef = Project.CurrentProject.ConditionDefinitions.FindById(conditionDefRef.ConditionDefinitionId).conditionDefinition;
                string paramText = conditionDef.ParamText;
                conditionDef.Parameters.Elements.ForEach(el =>
                {
                    var paramDef = (ParameterDefinition)el;
                    string type = paramDef.ReturnType.Type;
                    returnTypes.Add(type);
                });

                generated = RecurseGenerateParamText(paramText, conditionDefRef.function.parameters, returnTypes);
            }
            else
            {
                returnTypes = TriggerData.GetParameterReturnTypes(eca.function);
                string paramText = TriggerData.GetParamText(eca.function);
                generated = RecurseGenerateParamText(paramText, eca.function.parameters, returnTypes);
            }

            // First and last inline must be a string.
            // Otherwise hyperlinks get cut from the treeitem header (WPF black magic).
            Inlines.Add(new Run(""));
            Inlines.AddRange(generated);
            Inlines.Add(new Run(""));

            // Specially handled SetVariable
            if (eca is SetVariable)
            {
                Parameter setVarParam = hyperlinkParameters[0].parameter;
                var isVariableSetEmpty = !(setVarParam is VariableRef);
                if (isVariableSetEmpty)
                    hyperlinkParameters[1].Disable();
            }

            return Inlines;
        }

        /// <summary>
        /// Draws the parameter text with selectable hyperlinks for TriggerElements.
        /// </summary>
        /// <returns></returns>
        private List<Inline> RecurseGenerateParamText(string paramText, List<Parameter> parameters, List<string> returnTypes)
        {
            // TODO: parameters with no commas at the end of a param crashes this function.
            // 'SetPlayerTechResearchedSwap' as an example.

            List<Inline> inlines = new List<Inline>();

            int paramIndex = 0;
            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] != '~')
                {
                    stringBuilder.Append(paramText[i]);
                    continue;
                }

                if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == ',')
                    stringBuilder.Remove(stringBuilder.Length - 1, 1); // Removes comma before param

                inlines.Add(new Run(stringBuilder.ToString())
                {
                    FontFamily = TriggerEditorFont.GetParameterFont(),
                    FontSize = TriggerEditorFont.GetParameterFontSize()
                });
                stringBuilder.Clear();



                if (parameters[paramIndex] is Function)
                {
                    var function = (Function)parameters[paramIndex];
                    if (function.parameters.Count > 0) // first bracket gets hyperlinked
                    {
                        List<string> _returnTypes = TriggerData.GetParameterReturnTypes(function);
                        if (settings.triggerEditorMode == 0)
                            inlines.Add(AddHyperlink("(", parameters, paramIndex, returnTypes[paramIndex]));
                        else
                            inlines.Add(AddHyperlink(" ( ", parameters, paramIndex, returnTypes[paramIndex]));

                        inlines.AddRange(RecurseGenerateParamText(TriggerData.GetParamText(function), function.parameters, _returnTypes)); // recurse
                    }
                    else // whole displayname gets hyperlinked
                    {
                        Run runFirstBracket = new Run("(");
                        runFirstBracket.FontFamily = TriggerEditorFont.GetParameterFont();
                        runFirstBracket.FontSize = TriggerEditorFont.GetParameterFontSize();
                        inlines.Add(runFirstBracket);
                        inlines.Add(AddHyperlink(TriggerData.GetParamDisplayName(function), parameters, paramIndex, returnTypes[paramIndex]));
                    }
                    Run run = new Run(")");
                    run.FontFamily = TriggerEditorFont.GetParameterFont();
                    run.FontSize = TriggerEditorFont.GetParameterFontSize();
                    inlines.Add(run);
                }
                else if (parameters[paramIndex] is Preset)
                {
                    inlines.Add(AddHyperlink(TriggerData.GetParamDisplayName(parameters[paramIndex]), parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is VariableRef)
                {
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = project.Variables.GetByReference(variableRef, _explorerElement);
                    string varName = string.Empty;

                    string expectedType = null;
                    string actualType = null;
                    if (variable != null)
                    {
                        expectedType = Types.GetBaseType(returnTypes[paramIndex]);
                        actualType = Types.GetBaseType(variable.Type);
                    }

                    // This exists in case a variable has been changed
                    if (variable == null || !Types.AreTypesEqual(expectedType, actualType))
                        varName = "null";
                    else
                        varName = project.Variables.GetVariableNameById(variable.Id);

                    inlines.Add(AddHyperlink(varName, parameters, paramIndex, returnTypes[paramIndex]));
                    List<string> _returnTypes = new List<string>();
                    _returnTypes.Add("integer");
                    _returnTypes.Add("integer");
                    if (variable != null && variable.IsArray && !variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText("[,~Number,]", variableRef.arrayIndexValues, _returnTypes));
                    else if (variable != null && variable.IsArray && variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText("[,~Number,][,~Number,]", variableRef.arrayIndexValues, _returnTypes));
                }
                else if (parameters[paramIndex] is TriggerRef)
                {
                    var triggerRef = (TriggerRef)parameters[paramIndex];
                    var trigger = project.Triggers.GetByReference(triggerRef);
                    string triggerName = string.Empty;

                    // This exists in case a trigger name has been changed
                    if (trigger == null)
                        triggerName = "null";
                    else
                        triggerName = project.Triggers.GetName(trigger.trigger.Id);

                    inlines.Add(AddHyperlink(triggerName, parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is ParameterDefinitionRef paramDefRef)
                {
                    var parameterDefs = _explorerElement.GetParameterCollection();
                    var paramDef = parameterDefs.GetByReference(paramDefRef);
                    string name;

                    if (paramDef == null)
                        name = "null";
                    else
                        name = paramDef.Name;

                    inlines.Add(AddHyperlink(name, parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is Value)
                {
                    var value = (Value)parameters[paramIndex];
                    var name = project.Triggers.GetValueName(value.value, returnTypes[paramIndex]);

                    // This exists in case a variable has been changed
                    //if (name == null || name == "")
                    //{
                    //    parameters[paramIndex] = new Parameter();
                    //    name = "null";
                    //}
                    inlines.Add(AddHyperlink(name, parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set.
                {
                    i++; // avoids the '~' in the name
                    int startIndex = i; // store current letter index
                    int length = 0;
                    bool isParamNameSet = false;
                    while (!isParamNameSet && i < paramText.Length) // scan parameter display name
                    {
                        if (paramText[i] == ',')
                            isParamNameSet = true;
                        else
                        {
                            length++;
                            i++;
                        }
                    }
                    string paramName = paramText.Substring(startIndex, length);
                    inlines.Add(AddHyperlink(paramName, parameters, paramIndex, returnTypes[paramIndex]));
                }

                while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                {
                    i++;
                }
                paramIndex++;
            }

            inlines.Add(new Run(stringBuilder.ToString())
            {
                FontFamily = TriggerEditorFont.GetParameterFont(),
                FontSize = TriggerEditorFont.GetParameterFontSize()
            });
            stringBuilder.Clear();

            return inlines;
        }

        private HyperlinkBT AddHyperlink(string text, List<Parameter> parameters, int index, string returnType)
        {
            HyperlinkBT hyperlinkBT;
            if (_eca != null)
            {
                hyperlinkBT = new HyperlinkParameterTrigger(_explorerElement, _eca, text, parameters, index, returnType);
            }
            else
            {
                hyperlinkBT = new HyperlinkParameterVariable(_variable, parameters[0], text);
            }

            hyperlinkParameters.Add(hyperlinkBT);

            return hyperlinkBT;
        }
    }
}
