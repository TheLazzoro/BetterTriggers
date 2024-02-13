using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementReplace : ICommand
    {
        string commandName = "Replace Trigger Element";
        TriggerElement toReplace;
        TriggerElement toInsert;
        TriggerElementCollection parent;
        int insertIndex = 0;

        public CommandTriggerElementReplace(TriggerElement toReplace, TriggerElement toInsert)
        {
            this.toReplace = toReplace;
            this.toInsert = toInsert;
            this.parent = toReplace.GetParent();
            this.insertIndex = parent.IndexOf(toReplace);
        }

        public void Execute()
        {
            toReplace.RemoveFromParent();
            toInsert.SetParent(parent, insertIndex);
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            toReplace.RemoveFromParent();
            toInsert.SetParent(parent, insertIndex);
        }

        public void Undo()
        {
            toInsert.RemoveFromParent();
            toReplace.SetParent(parent, insertIndex);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
