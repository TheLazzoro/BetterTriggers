using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementRename : ICommand
    {
        string commandName = "Rename Local Variable";
        ParameterDefinition parameterDefinition;
        LocalVariable localVariable;
        string oldName;
        string newName;
        RefCollection refCollection;

        public CommandTriggerElementRename(LocalVariable localVariable, string newName)
        {
            this.localVariable = localVariable;
            this.oldName = localVariable.variable.Name;
            this.newName = newName;
            this.refCollection = new RefCollection(localVariable.variable);
        }

        public CommandTriggerElementRename(ParameterDefinition parameterDefinition, string newName)
        {
            this.parameterDefinition = parameterDefinition;
            this.oldName = parameterDefinition.Name;
            this.newName = newName;
            this.refCollection = new RefCollection(parameterDefinition);
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
            refCollection.Notify();
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
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
            refCollection.Notify();
        }

        public void Undo()
        {
            if (localVariable != null)
            {
                localVariable.variable.Name = oldName;
                localVariable.DisplayText = oldName;
            }
            else if (parameterDefinition != null)
            {
                parameterDefinition.Name = newName;
            }
            refCollection.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
