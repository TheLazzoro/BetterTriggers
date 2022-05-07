using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        TriggerElement triggerElement;
        int triggerId;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        // special case 'SetVariable'
        Parameter setVarValueOld;
        Parameter setVarValueNew;

        public CommandTriggerElementParamModify(TriggerElement triggerElement, int triggerId, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            this.triggerElement = triggerElement;
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.triggerId = triggerId;
            this.oldParameter = paramCollection[this.paramIndex];
        }

        public void Execute()
        {
            ControllerVariable controller = new ControllerVariable();

            if (paramToAdd is VariableRef)
            {
                var variableRef = paramToAdd as VariableRef;
                controller.SetReferenceToVariable(variableRef.VariableId, triggerId);
            }
            if (oldParameter is VariableRef)
            {
                var variableRef = oldParameter as VariableRef;
                controller.RemoveReferenceToVariable(variableRef.VariableId, triggerId);
            }
            else if (oldParameter is Function)
            {
                controller.RecurseAddVariableRefs((Function)oldParameter, triggerId);
            }

            paramCollection[paramIndex] = paramToAdd;

            // Special case
            if (triggerElement.function is SetVariable)
            {
                Parameter setVarParam = triggerElement.function.parameters[0];
                Parameter value = triggerElement.function.parameters[1];

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
            ControllerVariable controller = new ControllerVariable();

            if (paramToAdd is VariableRef)
            {
                var variableRef = paramToAdd as VariableRef;
                controller.SetReferenceToVariable(variableRef.VariableId, triggerId);
            }
            if (oldParameter is VariableRef)
            {
                var variableRef = oldParameter as VariableRef;
                controller.SetReferenceToVariable(variableRef.VariableId, triggerId);
            }
            else if (oldParameter is Function)
            {
                controller.RecurseAddVariableRefs((Function)oldParameter, triggerId);
            }

            // 'SetVariable' special case
            if(setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueNew;
            }

            paramCollection[paramIndex] = paramToAdd;
            triggerElement.ChangedParams();
        }

        public void Undo()
        {
            ControllerVariable controller = new ControllerVariable();

            if (paramToAdd is VariableRef)
            {
                var variableRef = paramToAdd as VariableRef;
                controller.RemoveReferenceToVariable(variableRef.VariableId, triggerId);
            }
            if (oldParameter is VariableRef)
            {
                var variableRef = oldParameter as VariableRef;
                controller.RemoveReferenceToVariable(variableRef.VariableId, triggerId);
            }
            else if (oldParameter is Function)
            {
                controller.RecurseRemoveVariableRefs((Function)oldParameter, triggerId);
            }

            // 'SetVariable' special case
            if (setVarValueNew != null)
            {
                paramCollection[paramIndex + 1] = setVarValueOld;
            }

            paramCollection[paramIndex] = oldParameter;
            triggerElement.ChangedParams();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
