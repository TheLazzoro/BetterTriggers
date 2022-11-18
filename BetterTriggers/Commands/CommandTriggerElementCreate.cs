using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        TriggerElement triggerElement;
        List<TriggerElement> parent;
        int insertIndex = 0;

        public CommandTriggerElementCreate(TriggerElement triggerElement, List<TriggerElement> parent, int insertIndex)
        {
            this.triggerElement = triggerElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            triggerElement.SetParent(parent, insertIndex);
            triggerElement.Created(insertIndex);
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);
            triggerElement.Created(insertIndex);
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.Deleted();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
