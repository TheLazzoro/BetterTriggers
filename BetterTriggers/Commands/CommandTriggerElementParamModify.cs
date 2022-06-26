using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

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
        Parameter setVarValueOld;
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
            if (triggerElement.function is SetVariable)
            {
                Parameter setVarParam = triggerElement.function.parameters[0];
                Parameter value = triggerElement.function.parameters[1];
                value.returnType = setVarParam.returnType;

                // Reset if value type doesn't match variable type
                if (paramCollection[paramIndex] == setVarParam && paramToAdd.identifier != value.identifier)
                {
                    setVarValueOld = value;
                    setVarValueNew = new Parameter()
                    {
                        identifier = null,
                        returnType = setVarParam.returnType,
                    };

                    paramCollection[paramIndex + 1] = setVarValueNew;
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
