using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        ExplorerElement explorerElement;
        TriggerElement triggerElement;
        TriggerElement OldParent;
        TriggerElement NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandTriggerElementMove(ExplorerElement explorerElement, TriggerElement triggerElement, TriggerElementCollection NewParent, int NewInsertIndex)
        {
            this.explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            this.OldParent = triggerElement.GetParent();
            this.OldInsertIndex = this.OldParent.IndexOf(triggerElement);
            this.NewParent = NewParent;
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            TriggerValidator validator = new TriggerValidator(explorerElement);
            validator.RemoveInvalidReferences(NewParent);
            Project.CurrentProject.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(OldParent, OldInsertIndex);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
