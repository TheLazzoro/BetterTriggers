using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        TriggerElement triggerElement;
        ExplorerElementTrigger explorerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld; // TODO: Why is this not used?
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(TriggerElement triggerElement, ExplorerElementTrigger explorerElement, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
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
            ControllerVariable controllerVar = new ControllerVariable();
            ControllerReferences controllerRef = new ControllerReferences();

            paramCollection[paramIndex] = paramToAdd;

            controllerRef.UpdateReferences(explorerElement);

            // Special case
            if (triggerElement is SetVariable)
            {
                Parameter setVarParam = triggerElement.function.parameters[0];
                Parameter value = triggerElement.function.parameters[1];

                if(paramCollection[paramIndex] == setVarParam && setVarParam is VariableRef && oldParameter is VariableRef)
                {
                    var setVarParamRef = setVarParam as VariableRef;
                    var setVarParamRefOld = oldParameter as VariableRef;
                    var newVar = controllerVar.GetByReference(setVarParamRef);
                    var oldVar = controllerVar.GetByReference(setVarParamRefOld);
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
            ControllerVariable controllerVar = new ControllerVariable();
            ControllerReferences controllerRef = new ControllerReferences();

            // 'SetVariable' special case
            if (setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueNew;
            }

            paramCollection[paramIndex] = paramToAdd;
            controllerRef.UpdateReferences(explorerElement);
            triggerElement.ChangedParams();
        }

        public void Undo()
        {
            ControllerVariable controllerVar = new ControllerVariable();
            ControllerReferences controllerRef = new ControllerReferences();

            paramCollection[paramIndex] = oldParameter;
            controllerRef.UpdateReferences(explorerElement);
            triggerElement.ChangedParams();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
