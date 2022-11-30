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
        RefCollection refCollection;

        public CommandVariableModifyInitialValue(Variable variable, Parameter parameter)
        {
            this.variable = variable;
            this.newParameter = parameter;
            this.oldParameter = variable.InitialValue;
            this.refCollection = new RefCollection(variable);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            References.UpdateReferences(variable);
            variable.InitialValue = newParameter;
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            References.UpdateReferences(variable);
            variable.InitialValue = newParameter;
        }

        public void Undo()
        {
            variable.InitialValue = oldParameter;
            refCollection.AddRefsToParent();
            References.UpdateReferences(variable);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
