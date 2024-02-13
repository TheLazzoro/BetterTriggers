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
        ECA triggerElement;
        ExplorerElement explorerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld; // TODO: Why is this not used?
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(ECA triggerElement, ExplorerElement explorerElement, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            this.triggerElement = triggerElement;
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.explorerElement = explorerElement;
            this.oldParameter = paramCollection[this.paramIndex];
        }

        public void Execute()
        {
            paramCollection[paramIndex] = paramToAdd;
            Project.CurrentProject.References.UpdateReferences(explorerElement);

            // Special case
            if (triggerElement is SetVariable)
            {
                Parameter setVarParam = triggerElement.function.parameters[0];
                Parameter value = triggerElement.function.parameters[1];

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
        }

        public void Undo()
        {
            paramCollection[paramIndex] = oldParameter;
            Project.CurrentProject.References.UpdateReferences(explorerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
