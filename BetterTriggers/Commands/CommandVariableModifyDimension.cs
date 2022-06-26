using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyDimension : ICommand
    {
        string commandName = "Modify Variable Dimension";
        ExplorerElementVariable explorerElement;
        bool isTwoDimensions;

        public CommandVariableModifyDimension(ExplorerElementVariable explorerElement, bool isTwoDimensions)
        {
            this.explorerElement = explorerElement;
            this.isTwoDimensions = isTwoDimensions;
        }

        public void Execute()
        {
            explorerElement.variable.IsTwoDimensions = isTwoDimensions;
            explorerElement.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.variable.IsTwoDimensions = isTwoDimensions;
            explorerElement.Notify();
        }

        public void Undo()
        {
            explorerElement.variable.IsTwoDimensions = !isTwoDimensions;
            explorerElement.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
