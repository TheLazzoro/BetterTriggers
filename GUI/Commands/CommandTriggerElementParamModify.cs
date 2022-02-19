using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Facades.Controllers;
using GUI.Components.TriggerEditor;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        int triggerId;
        TriggerElement triggerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        public CommandTriggerElementParamModify(int triggerId, TriggerElement triggerElement, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.triggerId = triggerId;
            this.triggerElement = triggerElement;
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
            this.triggerElement.FormatParameterText();

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

            paramCollection[paramIndex] = paramToAdd;
            this.triggerElement.FormatParameterText();
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

            paramCollection[paramIndex] = oldParameter;
            this.triggerElement.FormatParameterText();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
