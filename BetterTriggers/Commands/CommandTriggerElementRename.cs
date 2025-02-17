using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementRename : ICommand
    {
        string commandName = "Rename Local Variable";
        ExplorerElement explorerElement;
        ParameterDefinition parameterDefinition;
        LocalVariable localVariable;
        string oldName;
        string newName;
        RefCollection refCollection;

        public CommandTriggerElementRename(ExplorerElement explorerElement, LocalVariable localVariable, string newName)
        {
            this.explorerElement = explorerElement;
            this.localVariable = localVariable;
            this.oldName = localVariable.variable.Name;
            this.newName = newName;
            this.refCollection = new RefCollection(explorerElement, localVariable.variable);
        }

        public CommandTriggerElementRename(ExplorerElement explorerElement, ParameterDefinition parameterDefinition, string newName)
        {
            this.explorerElement = explorerElement;
            this.parameterDefinition = parameterDefinition;
            this.oldName = parameterDefinition.Name;
            this.newName = newName;
            this.refCollection = new RefCollection(explorerElement, parameterDefinition);
        }

        public void Execute()
        {
            if (localVariable != null)
            {
                localVariable.variable.Name = newName;
                localVariable.DisplayText = newName;
            }
            else if (parameterDefinition != null)
            {
                parameterDefinition.Name = newName;
            }
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            if (localVariable != null)
            {
                localVariable.variable.Name = newName;
                localVariable.DisplayText = newName;
                localVariable.IsSelected = true;
            }
            else if (parameterDefinition != null)
            {
                parameterDefinition.Name = newName;
                parameterDefinition.IsSelected = true;
            }
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();
        }

        public void Undo()
        {
            if (localVariable != null)
            {
                localVariable.variable.Name = oldName;
                localVariable.DisplayText = oldName;
                localVariable.IsSelected = true;
            }
            else if (parameterDefinition != null)
            {
                parameterDefinition.Name = oldName;
                parameterDefinition.IsSelected = true;
            }
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
