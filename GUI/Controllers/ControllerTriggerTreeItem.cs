using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using GUI.Components.TriggerEditor;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Controllers
{
    public class ControllerTriggerTreeItem
    {
        TreeViewTriggerElement treeItem;
        TextBlock textBlock;
        List<HyperlinkParameter> hyperlinkParameters = new List<HyperlinkParameter>();
        ControllerTrigger controllerTrigger = new ControllerTrigger();

        public ControllerTriggerTreeItem(TreeViewTriggerElement treeViewTriggerElement)
        {
            this.treeItem = treeViewTriggerElement;
            this.textBlock = treeViewTriggerElement.paramTextBlock;
        }

        public void GenerateParamText()
        {
            textBlock.Inlines.Clear();
            var inlines = RecurseGenerateParamText(treeItem.triggerElement.function.parameters, treeItem.paramText);

            // First and last inline must be a string.
            // Otherwise hyperlinks get cut from the treeitem header (WPF black magic).
            textBlock.Inlines.Add(new Run(""));
            textBlock.Inlines.AddRange(inlines);
            textBlock.Inlines.Add(new Run(""));

            // Specially handled SetVariable
            if (treeItem.triggerElement.function is SetVariable)
            {
                SetVariable setVariable = (SetVariable)treeItem.triggerElement.function;

                HyperlinkParameter[] topLayerParams = new HyperlinkParameter[2];
                int index = 0;
                int i = 0;
                while (i < hyperlinkParameters.Count && index < 2)
                {
                    var hyperlink = hyperlinkParameters[i];
                    if (hyperlink.parameters[hyperlink.index] == setVariable.parameters[index])
                    {
                        topLayerParams[index] = hyperlinkParameters[index];
                        index++;
                    }
                    i++;
                }

                Parameter setVarParam = topLayerParams[0].parameters[topLayerParams[0].index];
                var isVariableSetEmpty = !(setVarParam is VariableRef);
                if (isVariableSetEmpty)
                    topLayerParams[1].Disable();
            }
        }

        /// <summary>
        /// Draws the parameter text with selectable hyperlinks for TriggerElements.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="paramText"></param>
        /// <returns></returns>
        private List<Inline> RecurseGenerateParamText(List<Parameter> parameters, string paramText)
        {
            // TODO: parameters with no commas at the end of a param crashes this function.
            // 'SetPlayerTechResearchedSwap' as an example.


            List<Inline> inlines = new List<Inline>();
            ControllerTriggerData controller = new ControllerTriggerData();

            int paramIndex = 0;
            for (int i = 0; i < paramText.Length; i++)
            {
                if (paramText[i] != '~')
                {
                    inlines.Add(new Run(paramText[i].ToString())
                    {
                        FontFamily = new FontFamily("Verdana")
                    });
                    continue;
                }

                RemoveCommaBeforeParamIndicator(inlines);
                if (parameters[paramIndex] is Function)
                {
                    var function = (Function)parameters[paramIndex];
                    if (function.parameters.Count > 0) // first bracket gets hyperlinked
                    {
                        inlines.Add(AddHyperlink(treeItem, "(", parameters, paramIndex));
                        inlines.AddRange(RecurseGenerateParamText(function.parameters, controller.GetParamText(function))); // recurse
                    }
                    else // whole displayname gets hyperlinked
                    {
                        Run runFirstBracket = new Run("(");
                        runFirstBracket.FontFamily = new FontFamily("Verdana");
                        inlines.Add(runFirstBracket);
                        inlines.Add(AddHyperlink(treeItem, controller.GetParamDisplayName(function), parameters, paramIndex));
                    }
                    Run run = new Run(")");
                    run.FontFamily = new FontFamily("Verdana");
                    inlines.Add(run);
                }
                else if (parameters[paramIndex] is Constant)
                {
                    inlines.Add(AddHyperlink(treeItem, controller.GetParamDisplayName(parameters[paramIndex]), parameters, paramIndex));
                }
                else if (parameters[paramIndex] is VariableRef)
                {
                    var controllerVariable = new ControllerVariable();
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = controllerVariable.GetByReference(variableRef);
                    string varName = string.Empty;

                    var type = variableRef.returnType == "integervar" ? "integer" : variableRef.returnType; // hack

                    // This exists in case a variable has been changed
                    if (variable == null || variable.Type != type)
                    {
                        parameters[paramIndex] = new Parameter()
                        {
                            returnType = variableRef.returnType,
                        };
                        varName = "null";
                    } else
                        varName = ContainerVariables.GetVariableNameById(variable.Id);

                    inlines.Add(AddHyperlink(treeItem, varName, parameters, paramIndex));

                    if (variable != null && variable.IsArray && !variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(variableRef.arrayIndexValues, "[,~Number,]"));
                    else if (variable != null && variable.IsArray && variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(variableRef.arrayIndexValues, "[,~Number,][,~Number,]"));
                }
                else if (parameters[paramIndex] is TriggerRef)
                {
                    var controllerTrig = new ControllerTrigger();
                    var triggerRef = (TriggerRef)parameters[paramIndex];
                    var trigger = controllerTrig.GetByReference(triggerRef);
                    string triggerName = string.Empty;

                    // This exists in case a trigger name has been changed
                    if (trigger == null)
                    {
                        parameters[paramIndex] = new Parameter()
                        {
                            returnType = triggerRef.returnType,
                        };
                        triggerName = "null";
                    }
                    else
                        triggerName = controllerTrig.GetTriggerName(trigger.Id);

                    inlines.Add(AddHyperlink(treeItem, triggerName, parameters, paramIndex));
                }
                else if (parameters[paramIndex] is Value)
                {
                    // TODO: This will crash if a referenced variable is deleted.
                    var value = (Value)parameters[paramIndex];
                    var name = controllerTrigger.GetValueName(value.identifier, value.returnType);

                    // This exists in case a variable has been changed
                    if (name == null || name == "" || value.returnType != parameters[paramIndex].returnType)
                    {
                        parameters[paramIndex] = new Parameter()
                        {
                            returnType = value.returnType,
                        };
                        name = "null";
                    }
                    inlines.Add(AddHyperlink(treeItem, name, parameters, paramIndex));
                }
                else if (parameters[paramIndex] is Parameter) // In other words, parameter has not yet been set. Redundant?
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
                    inlines.Add(AddHyperlink(treeItem, paramName, parameters, paramIndex));
                }

                while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                {
                    i++;
                }
                paramIndex++;
            }

            return inlines;
        }

        private HyperlinkParameter AddHyperlink(TreeViewTriggerElement treeViewTriggerElement, string text, List<Parameter> parameters, int index)
        {
            HyperlinkParameter hyperlink = new HyperlinkParameter(treeViewTriggerElement, text, parameters, index);
            hyperlinkParameters.Add(hyperlink);

            return hyperlink;
        }

        private void RemoveCommaBeforeParamIndicator(List<Inline> inlines)
        {
            if (inlines.Count > 0 && inlines[inlines.Count - 1] != null) // apparently this is dangerous and crashes if not checked.
            {
                // gets the last inserted text range. In this case we check if in fact it's a commma.
                var textrange = new TextRange(inlines[inlines.Count - 1].ContentStart, inlines[inlines.Count - 1].ContentEnd);
                if (textrange.Text == ",")
                    inlines.Remove(inlines[inlines.Count - 1]); // removes the comma before the '~' indicator
            }
        }
    }
}
