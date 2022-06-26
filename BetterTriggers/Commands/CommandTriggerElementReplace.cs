using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementReplace : ICommand
    {
        string commandName = "Replace Trigger Element";
        TriggerElement toReplace;
        TriggerElement toInsert;
        List<TriggerElement> parent;
        int insertIndex = 0;

        public CommandTriggerElementReplace(TriggerElement toReplace, TriggerElement toInsert)
        {
            this.toReplace = toReplace;
            this.toInsert = toInsert;
            this.parent = toReplace.Parent;
            this.insertIndex = parent.IndexOf(toReplace);
        }

        public void Execute()
        {
            toReplace.RemoveFromParent();
            toReplace.Deleted();
            toInsert.SetParent(parent, insertIndex);
            toInsert.Created(insertIndex);
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            toReplace.RemoveFromParent();
            toReplace.Deleted();
            toInsert.SetParent(parent, insertIndex);
            toInsert.Created(insertIndex);
        }

        public void Undo()
        {
            toInsert.RemoveFromParent();
            toInsert.Deleted();
            toReplace.SetParent(parent, insertIndex);
            toReplace.Created(insertIndex);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
