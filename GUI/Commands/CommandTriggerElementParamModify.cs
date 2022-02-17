using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandTriggerElementParamModify : ICommand
    {
        string commandName = "Modify Parameter";
        TriggerElement triggerElement;
        List<Parameter> paramCollection;
        Parameter paramToAdd;
        Parameter oldParameter;
        int paramIndex = 0;

        public CommandTriggerElementParamModify(TriggerElement triggerElement, List<Parameter> paramCollection, int paramIndex, Parameter paramToAdd)
        {
            this.paramCollection = paramCollection;
            this.paramIndex = paramIndex;
            this.paramToAdd = paramToAdd;

            this.triggerElement = triggerElement;
            this.oldParameter = paramCollection[this.paramIndex];
        }

        public void Execute()
        {
            paramCollection[paramIndex] = paramToAdd;
            this.triggerElement.FormatParameterText(triggerElement.paramTextBlock, triggerElement.function.parameters);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            paramCollection[paramIndex] = paramToAdd;
            this.triggerElement.FormatParameterText(triggerElement.paramTextBlock, triggerElement.function.parameters);
        }

        public void Undo()
        {
            paramCollection[paramIndex] = oldParameter;
            this.triggerElement.FormatParameterText(triggerElement.paramTextBlock, triggerElement.function.parameters);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
