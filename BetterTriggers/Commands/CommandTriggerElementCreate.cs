using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        ExplorerElement _explorerElement;
        TriggerElement triggerElement;
        TriggerElement parent;
        int insertIndex = 0;

        public CommandTriggerElementCreate(ExplorerElement explorerElement, TriggerElement triggerElement, TriggerElement parent, int insertIndex)
        {
            _explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            triggerElement.SetParent(parent, insertIndex);
            Project.CurrentProject.CommandManager.AddCommand(this);
            _explorerElement.InvokeChange();
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);
            _explorerElement.InvokeChange();
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            _explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
