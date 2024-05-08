using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyDimension : ICommand
    {
        string commandName = "Modify Variable Dimension";
        Variable variable;
        bool isTwoDimensions;
        RefCollection refCollection;

        public CommandVariableModifyDimension(Variable variable, bool isTwoDimensions)
        {
            this.variable = variable;
            this.isTwoDimensions = isTwoDimensions;
            this.refCollection = new RefCollection(variable);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            variable.IsTwoDimensions = isTwoDimensions;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            variable.IsTwoDimensions = isTwoDimensions;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            variable.IsTwoDimensions = !isTwoDimensions;
            refCollection.AddRefsToParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
