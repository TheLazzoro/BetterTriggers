using BetterTriggers;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using GUI.Components.Shared;
using GUI.Components.TriggerEditor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Controllers
{
    public class ControllerParamText
    {
        List<HyperlinkBT> hyperlinkParameters = new List<HyperlinkBT>();
        TreeViewTriggerElement treeItem;

        StringBuilder stringBuilder = new StringBuilder();
        private Settings settings;

        public ControllerParamText()
        {
            settings = Settings.Load();
        }

        public string GenerateTreeItemText(TreeViewTriggerElement treeItem)
        {
            StringBuilder sb = new StringBuilder();
            var triggerElement = (ECA)treeItem.triggerElement;
            List<string> returnTypes = TriggerData.GetParameterReturnTypes(triggerElement.function);
            GenerateTreeItemText(sb, triggerElement.function.parameters, returnTypes, treeItem.paramText);

            return sb.ToString();
        }

        /// <summary>
        /// TODO: Probably fix copy-paste?
        /// 
        /// This is copy-pasted from 'RecurseGenerateParamText' and uses strings instead of UI components.
        /// It is slow to pull text out of 'Inline' UI components, so this serves as an optimized way
        /// to generate the header text for TreeViewItems when loading in large quantities.
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
                        GenerateTreeItemText(sb, function.parameters, _returnTypes, ControllerTriggerData.GetParamText(function)); // recurse
                    }
                    else // whole displayname gets hyperlinked
                        sb.Append($"({ControllerTriggerData.GetParamDisplayName(function)}");

                    sb.Append(")");
                }
                else if (parameters[paramIndex] is Constant)
                    sb.Append(ControllerTriggerData.GetParamDisplayName(parameters[paramIndex]));

                else if (parameters[paramIndex] is VariableRef)
                {
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = ControllerVariable.GetByReference(variableRef);
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
                        varName = Variables.GetVariableNameById(variable.Id);

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
                    var trigger = ControllerTrigger.GetByReference(triggerRef);
                    string triggerName = string.Empty;

                    // This exists in case a trigger name has been changed
                    if (trigger == null)
                        triggerName = "null";
                    else
                        triggerName = ControllerTrigger.GetTriggerName(trigger.Id);

                    sb.Append(triggerName);
                }
                else if (parameters[paramIndex] is Value)
                {
                    var value = (Value)parameters[paramIndex];
                    var name = ControllerTrigger.GetValueName(value.value, returnTypes[paramIndex]);

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
            List<Inline> Inlines = new List<Inline>();
            var generated = RecurseGenerateParamText(new ParameterFacadeVariable(variable, "~Value"));

            // First and last inline must be a string.
            // Otherwise hyperlinks get cut from the treeitem header (WPF black magic).
            Inlines.Add(new Run(""));
            Inlines.AddRange(generated);
            Inlines.Add(new Run(""));

            return Inlines;
        }

        public List<Inline> GenerateParamText(TreeViewTriggerElement treeItem)
        {
            this.treeItem = treeItem;
            var triggerElement = (ECA)treeItem.triggerElement;
            List<Inline> Inlines = new List<Inline>();
            List<string> returnTypes = TriggerData.GetParameterReturnTypes(triggerElement.function);
            var generated = RecurseGenerateParamText(new ParameterFacadeTrigger(treeItem, triggerElement.function.parameters, returnTypes, treeItem.paramText));

            // First and last inline must be a string.
            // Otherwise hyperlinks get cut from the treeitem header (WPF black magic).
            Inlines.Add(new Run(""));
            Inlines.AddRange(generated);
            Inlines.Add(new Run(""));

            // Specially handled SetVariable
            if (treeItem.triggerElement is SetVariable)
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
        private List<Inline> RecurseGenerateParamText(IParameterFacade parameterFacade)
        {
            // TODO: parameters with no commas at the end of a param crashes this function.
            // 'SetPlayerTechResearchedSwap' as an example.

            string paramText = parameterFacade.GetParameterText();
            List<Parameter> parameters = parameterFacade.GetParametersAll();
            List<string> returnTypes = parameterFacade.GetReturnTypes();

            List<Inline> inlines = new List<Inline>();
            ControllerTriggerData controller = new();

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
                        if(settings.triggerEditorMode == 0)
                            inlines.Add(AddHyperlink(parameterFacade, "(", parameters, paramIndex, returnTypes[paramIndex]));
                        else
                            inlines.Add(AddHyperlink(parameterFacade, " ( ", parameters, paramIndex, returnTypes[paramIndex]));

                        inlines.AddRange(RecurseGenerateParamText(new ParameterFacadeTrigger(treeItem, function.parameters, _returnTypes, ControllerTriggerData.GetParamText(function)))); // recurse
                    }
                    else // whole displayname gets hyperlinked
                    {
                        Run runFirstBracket = new Run("(");
                        runFirstBracket.FontFamily = TriggerEditorFont.GetParameterFont();
                        runFirstBracket.FontSize = TriggerEditorFont.GetParameterFontSize();
                        inlines.Add(runFirstBracket);
                        inlines.Add(AddHyperlink(parameterFacade, ControllerTriggerData.GetParamDisplayName(function), parameters, paramIndex, returnTypes[paramIndex]));
                    }
                    Run run = new Run(")");
                    run.FontFamily = TriggerEditorFont.GetParameterFont();
                    run.FontSize = TriggerEditorFont.GetParameterFontSize();
                    inlines.Add(run);
                }
                else if (parameters[paramIndex] is Constant)
                {
                    inlines.Add(AddHyperlink(parameterFacade, ControllerTriggerData.GetParamDisplayName(parameters[paramIndex]), parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is VariableRef)
                {
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = ControllerVariable.GetByReference(variableRef);
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
                        varName = Variables.GetVariableNameById(variable.Id);

                    inlines.Add(AddHyperlink(parameterFacade, varName, parameters, paramIndex, returnTypes[paramIndex]));
                    List<string> _returnTypes = new List<string>();
                    _returnTypes.Add("integer");
                    _returnTypes.Add("integer");
                    if (variable != null && variable.IsArray && !variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(new ParameterFacadeTrigger(treeItem, variableRef.arrayIndexValues, _returnTypes, "[,~Number,]")));
                    else if (variable != null && variable.IsArray && variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(new ParameterFacadeTrigger(treeItem, variableRef.arrayIndexValues, _returnTypes, "[,~Number,][,~Number,]")));
                }
                else if (parameters[paramIndex] is TriggerRef)
                {
                    var triggerRef = (TriggerRef)parameters[paramIndex];
                    var trigger = ControllerTrigger.GetByReference(triggerRef);
                    string triggerName = string.Empty;

                    // This exists in case a trigger name has been changed
                    if (trigger == null)
                        triggerName = "null";
                    else
                        triggerName = ControllerTrigger.GetTriggerName(trigger.Id);

                    inlines.Add(AddHyperlink(parameterFacade, triggerName, parameters, paramIndex, returnTypes[paramIndex]));
                }
                else if (parameters[paramIndex] is Value)
                {
                    var value = (Value)parameters[paramIndex];
                    var name = ControllerTrigger.GetValueName(value.value, returnTypes[paramIndex]);

                    // This exists in case a variable has been changed
                    //if (name == null || name == "")
                    //{
                    //    parameters[paramIndex] = new Parameter();
                    //    name = "null";
                    //}
                    inlines.Add(AddHyperlink(parameterFacade, name, parameters, paramIndex, returnTypes[paramIndex]));
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
                    inlines.Add(AddHyperlink(parameterFacade, paramName, parameters, paramIndex, returnTypes[paramIndex]));
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

        private HyperlinkBT AddHyperlink(IParameterFacade parameterFacade, string text, List<Parameter> parameters, int index, string returnType)
        {
            HyperlinkBT hyperlinkBT = null;
            if (parameterFacade is ParameterFacadeTrigger)
                hyperlinkBT = new HyperlinkParameterTrigger(parameterFacade as ParameterFacadeTrigger, text, parameters, index, returnType);
            else if (parameterFacade is ParameterFacadeVariable)
                hyperlinkBT = new HyperlinkParameterVariable(parameterFacade as ParameterFacadeVariable, text);

            hyperlinkParameters.Add(hyperlinkBT);

            return hyperlinkBT;
        }
    }
}
