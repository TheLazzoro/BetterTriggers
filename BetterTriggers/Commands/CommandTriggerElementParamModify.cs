using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        ECA eca;
        ExplorerElement explorerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld; // TODO: Why is this not used?
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(ExplorerElement explorerElement, ECA eca, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            this.explorerElement = explorerElement;
            this.eca = eca;
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.oldParameter = paramCollection[this.paramIndex];
        }

        public void Execute()
        {
            paramCollection[paramIndex] = paramToAdd;
            Project.CurrentProject.References.UpdateReferences(explorerElement);

            // Special case
            if (eca is SetVariable)
            {
                Parameter setVarParam = eca.function.parameters[0];
                Parameter value = eca.function.parameters[1];

                if(paramCollection[paramIndex] == setVarParam && setVarParam is VariableRef && oldParameter is VariableRef)
                {
                    int i = 0;
                    var setVarParamRef = setVarParam as VariableRef;
                    var setVarParamRefOld = oldParameter as VariableRef;
                    var newVar = Project.CurrentProject.Variables.GetByReference(setVarParamRef);
                    var oldVar = Project.CurrentProject.Variables.GetByReference(setVarParamRefOld);
                    if(!Types.AreTypesEqual(newVar.Type, oldVar.Type))
                    {
                        setVarValueOld = value;
                        setVarValueNew = new Parameter()
                        {
                            value = null,
                        };
                        paramCollection[paramIndex + 1] = setVarValueNew;
                    }

                    // Copy array index parameters
                    if(newVar.IsArray == oldVar.IsArray && newVar.IsTwoDimensions == oldVar.IsTwoDimensions)
                    {
                        setVarParamRef.arrayIndexValues[0] = setVarParamRefOld.arrayIndexValues[0].Clone();
                        setVarParamRef.arrayIndexValues[1] = setVarParamRefOld.arrayIndexValues[1].Clone();
                    }
                }
            }

            explorerElement.InvokeChange();

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            // 'SetVariable' special case
            if (setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueNew;
            }

            paramCollection[paramIndex] = paramToAdd;
            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            paramCollection[paramIndex] = oldParameter;
            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
