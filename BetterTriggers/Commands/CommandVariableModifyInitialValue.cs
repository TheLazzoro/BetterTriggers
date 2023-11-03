using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyInitialValue : ICommand
    {
        string commandName = "Modify Initial Value";
        Variable variable;
        Parameter newParameter;
        Parameter oldParameter;

        public CommandVariableModifyInitialValue(Variable variable, Parameter parameter)
        {
            this.variable = variable;
            this.newParameter = parameter;
            this.oldParameter = variable.InitialValue;
        }

        public void Execute()
        {
            variable.InitialValue = newParameter;
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.InitialValue = newParameter;
        }

        public void Undo()
        {
            variable.InitialValue = oldParameter;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
