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

        public CommandLocalVariableRename(LocalVariable localVariable, string newName)
        {
            this.localVariable = localVariable;
            this.oldName = localVariable.variable.Name;
            this.newName = newName;
        }

        public void Execute()
        {
            localVariable.variable.Name = newName;
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            localVariable.variable.Name = newName;
        }

        public void Undo()
        {
            localVariable.variable.Name = oldName;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
