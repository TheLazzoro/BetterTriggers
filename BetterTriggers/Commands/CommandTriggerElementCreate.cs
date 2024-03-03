using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        TriggerElement triggerElement;
        TriggerElement parent;
        int insertIndex = 0;

        public CommandTriggerElementCreate(TriggerElement triggerElement, TriggerElement parent, int insertIndex)
        {
            this.triggerElement = triggerElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            triggerElement.SetParent(parent, insertIndex);
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
