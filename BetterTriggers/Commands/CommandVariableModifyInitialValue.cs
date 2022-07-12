using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyInitialValue : ICommand
    {
        string commandName = "Modify Initial Value";
        ExplorerElementVariable explorerElement;
        Parameter newParameter;
        Parameter oldParameter;

        public CommandVariableModifyInitialValue(ExplorerElementVariable explorerElement, Parameter parameter)
        {
            this.explorerElement = explorerElement;
            this.newParameter = parameter;
            this.oldParameter = explorerElement.variable.InitialValue;
        }

        public void Execute()
        {
            explorerElement.variable.InitialValue = newParameter;
            explorerElement.OnRemoteChange();
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.InitialValue = newParameter;
            explorerElement.OnRemoteChange();
        }

        public void Undo()
        {
            explorerElement.variable.InitialValue = oldParameter;
            explorerElement.OnRemoteChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
