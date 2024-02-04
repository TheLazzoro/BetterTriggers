using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        Trigger_Saveable trig;
        TriggerElement_Saveable triggerElement;
        List<TriggerElement_Saveable> OldParent;
        List<TriggerElement_Saveable> NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandTriggerElementMove(Trigger_Saveable trig, TriggerElement_Saveable triggerElement, List<TriggerElement_Saveable> NewParent, int NewInsertIndex)
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
            triggerElement.ChangedPosition();
            Project.CurrentProject.Triggers.RemoveInvalidReferences(trig, new List<TriggerElement_Saveable>() { triggerElement });
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            triggerElement.ChangedPosition();
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(OldParent, OldInsertIndex);
            triggerElement.ChangedPosition();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
