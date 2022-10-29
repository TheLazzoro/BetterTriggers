using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyArray : ICommand
    {
        string commandName = "Modify Variable Array";
        Variable variable;
        bool isArray;

        public CommandVariableModifyArray(Variable variable, bool isArray)
        {
            this.variable = variable;
            this.isArray = isArray;
        }

        public void Execute()
        {
            variable.IsArray = isArray;

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.IsArray = isArray;
        }

        public void Undo()
        {
            variable.IsArray = !isArray;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
