using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        string commandName = "Modify Variable Type";
        ExplorerElement explorerElement;
        Variable variable;
        War3Type selectedType;
        War3Type previousType;
        Parameter newInitialValue;
        Parameter previousInitialValue;
        RefCollection refCollection;

        public CommandVariableModifyType(ExplorerElement explorerElement, Variable variable, War3Type selectedType)
        {
            this.explorerElement = explorerElement;
            this.variable = variable;
            this.selectedType = selectedType;
            this.previousType = variable.War3Type;
            this.previousInitialValue = variable.InitialValue;
            this.newInitialValue = new Parameter();
            this.refCollection = new RefCollection(variable, selectedType);
        }

        public void Execute()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = previousInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = previousType;
            refCollection.AddRefsToParent();
            Project.CurrentProject.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
