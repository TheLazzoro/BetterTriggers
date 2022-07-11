using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        string commandName = "Modify Variable Type";
        ExplorerElementVariable explorerElement;
        string selectedType;
        string previousType;
        Parameter newInitialValue;
        Parameter previousInitialValue;

        public CommandVariableModifyType(ExplorerElementVariable explorerElement, string selectedType, string previousType)
        {
            this.explorerElement = explorerElement;
            this.selectedType = selectedType;
            this.previousType = previousType;
            this.previousInitialValue = explorerElement.variable.InitialValue;
            this.newInitialValue = new Parameter();
        }

        public void Execute()
        {
            explorerElement.variable.InitialValue = newInitialValue;
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.InitialValue = newInitialValue;
            explorerElement.variable.Type = selectedType;
            explorerElement.Notify();
        }

        public void Undo()
        {
            explorerElement.variable.InitialValue = previousInitialValue;
            explorerElement.variable.Type = previousType;
            explorerElement.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
