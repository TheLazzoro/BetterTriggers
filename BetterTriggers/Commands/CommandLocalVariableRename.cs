using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandLocalVariableRename : ICommand
    {
        string commandName = "Rename Local Variable";
        LocalVariable localVariable;
        string oldName;
        string newName;
        RefCollection refCollection;

        public CommandLocalVariableRename(LocalVariable localVariable, string newName)
        {
            this.localVariable = localVariable;
            this.oldName = localVariable.variable.Name;
            this.newName = newName;
            this.refCollection = new RefCollection(localVariable.variable);
        }

        public void Execute()
        {
            localVariable.variable.Name = newName;
            localVariable.DisplayText = newName;
            refCollection.Notify();
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            localVariable.variable.Name = newName;
            localVariable.DisplayText = newName;
            refCollection.Notify();
        }

        public void Undo()
        {
            localVariable.variable.Name = oldName;
            localVariable.DisplayText = oldName;
            refCollection.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
