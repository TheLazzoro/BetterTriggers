using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        ECA triggerElement;
        ExplorerElementTrigger explorerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld; // TODO: Why is this not used?
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(ECA triggerElement, ExplorerElementTrigger explorerElement, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
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
            References.UpdateReferences(explorerElement);

            // Special case
            if (triggerElement is SetVariable)
            {
                Parameter setVarParam = triggerElement.function.parameters[0];
                Parameter value = triggerElement.function.parameters[1];

                if(paramCollection[paramIndex] == setVarParam && setVarParam is VariableRef && oldParameter is VariableRef)
                {
                    var setVarParamRef = setVarParam as VariableRef;
                    var setVarParamRefOld = oldParameter as VariableRef;
                    var newVar = ControllerVariable.GetByReference(setVarParamRef);
                    var oldVar = ControllerVariable.GetByReference(setVarParamRefOld);
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

            triggerElement.ChangedParams();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            // 'SetVariable' special case
            if (setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueNew;
            }

            paramCollection[paramIndex] = paramToAdd;
            References.UpdateReferences(explorerElement);
            triggerElement.ChangedParams();
        }

        public void Undo()
        {
            paramCollection[paramIndex] = oldParameter;
            References.UpdateReferences(explorerElement);
            triggerElement.ChangedParams();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
