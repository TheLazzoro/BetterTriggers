using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyArray : ICommand
    {
        string commandName = "Modify Variable Array";
        ExplorerElementVariable explorerElement;
        bool isArray;

        public CommandVariableModifyArray(ExplorerElementVariable explorerElement, bool isArray)
        {
            this.explorerElement = explorerElement;
            this.isArray = isArray;
        }

        public void Execute()
        {
            explorerElement.variable.IsArray = isArray;
            explorerElement.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.IsArray = isArray;
            explorerElement.Notify();
        }

        public void Undo()
        {
            explorerElement.variable.IsArray = !isArray;
            explorerElement.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
