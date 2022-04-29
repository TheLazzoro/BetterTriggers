using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using GUI.Components.TriggerEditor;
using Model.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Controllers
{
    public class ControllerTriggerTreeItem
    {
        TreeViewTriggerElement treeItem;
        TextBlock textBlock;

        public ControllerTriggerTreeItem(TreeViewTriggerElement treeViewTriggerElement)
        {
            this.treeItem = treeViewTriggerElement;
            this.textBlock = treeViewTriggerElement.paramTextBlock;
        }

        public void GenerateParamText()
        {
            textBlock.Inlines.Clear();
            var inlines = RecurseGenerateParamText(treeItem.triggerElement.function.parameters, treeItem.paramText);
            textBlock.Inlines.AddRange(inlines);
        }

        private List<Inline> RecurseGenerateParamText(List<Parameter> parameters, string paramText)
        {
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
                        inlines.Add(new HyperlinkParameter(treeItem, "(", parameters, paramIndex));
                        inlines.AddRange(RecurseGenerateParamText(function.parameters, controller.GetParamText(function))); // recurse
                    }
                    else // whole displayname gets hyperlinked
                    {
                        Run runFirstBracket = new Run("(");
                        runFirstBracket.FontFamily = new FontFamily("Verdana");
                        inlines.Add(runFirstBracket);
                        inlines.Add(new HyperlinkParameter(treeItem, controller.GetParamDisplayName(function), parameters, paramIndex));
                    }
                    Run run = new Run(")");
                    run.FontFamily = new FontFamily("Verdana");
                    inlines.Add(run);
                }
                else if (parameters[paramIndex] is Constant)
                {
                    inlines.Add(new HyperlinkParameter(treeItem, controller.GetParamDisplayName(parameters[paramIndex]), parameters, paramIndex));
                }
                else if (parameters[paramIndex] is VariableRef)
                {
                    // TODO: This will crash if a referenced variable is deleted.
                    var variableRef = (VariableRef)parameters[paramIndex];
                    var variable = ContainerVariables.GetVariableById(variableRef.VariableId);
                    var varName = ContainerVariables.GetVariableNameById(variable.Id);

                    // This exists in case a variable has been changed
                    if (varName == null || varName == "" || variable.Type != parameters[paramIndex].returnType)
                    {
                        parameters[paramIndex] = new Parameter()
                        {
                            returnType = variableRef.returnType,
                        };
                        varName = "null";
                    }
                    inlines.Add(new HyperlinkParameter(treeItem, varName, parameters, paramIndex));

                    if (variable.IsArray && !variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(variableRef.arrayIndexValues, "[,~Number,]"));
                    else if (variable.IsArray && variable.IsTwoDimensions)
                        inlines.AddRange(RecurseGenerateParamText(variableRef.arrayIndexValues, "[,~Number,][,~Number,]"));
                }
                else if (parameters[paramIndex] is Value)
                {
                    // TODO: This will crash if a referenced variable is deleted.
                    var value = (Value)parameters[paramIndex];
                    var name = value.identifier;

                    // This exists in case a variable has been changed
                    if (name == null || name == "" || value.returnType != parameters[paramIndex].returnType)
                    {
                        parameters[paramIndex] = new Parameter()
                        {
                            returnType = value.returnType,
                        };
                        name = "null";
                    }
                    inlines.Add(new HyperlinkParameter(treeItem, name, parameters, paramIndex));
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
                    inlines.Add(new HyperlinkParameter(treeItem, paramName, parameters, paramIndex));
                }

                while (i < paramText.Length && paramText[i] != ',') // erases placeholder param name
                {
                    i++;
                }
                paramIndex++;
            }

            return inlines;
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
