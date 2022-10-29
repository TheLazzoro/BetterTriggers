using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyDimension : ICommand
    {
        string commandName = "Modify Variable Dimension";
        Variable variable;
        bool isTwoDimensions;

        public CommandVariableModifyDimension(Variable variable, bool isTwoDimensions)
        {
            this.variable = variable;
            this.isTwoDimensions = isTwoDimensions;
        }

        public void Execute()
        {
            variable.IsTwoDimensions = isTwoDimensions;

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.IsTwoDimensions = isTwoDimensions;
        }

        public void Undo()
        {
            variable.IsTwoDimensions = !isTwoDimensions;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
