using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        Trigger trig;
        TriggerElement triggerElement;
        TriggerElement OldParent;
        TriggerElement NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandTriggerElementMove(Trigger trig, TriggerElement triggerElement, TriggerElementCollection NewParent, int NewInsertIndex)
        {
            this.trig = trig;
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
            Project.CurrentProject.Triggers.RemoveInvalidReferences(trig, NewParent);
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(OldParent, OldInsertIndex);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
