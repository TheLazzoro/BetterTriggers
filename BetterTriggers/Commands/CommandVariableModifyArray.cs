using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyArray : ICommand
    {
        string commandName = "Modify Variable Array";
        Variable variable;
        bool isArray;
        RefCollection refCollection;

        public CommandVariableModifyArray(Variable variable, bool isArray)
        {
            this.variable = variable;
            this.isArray = isArray;
            this.refCollection = new RefCollection(variable);
        }

        public void Execute()
        {
            var project = Project.CurrentProject;
            refCollection.RemoveRefsFromParent();
            project.References.UpdateReferences(variable);
            variable.IsArray = isArray;
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());

            project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            variable.IsArray = isArray;
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            variable.IsArray = !isArray;
            refCollection.AddRefsToParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
