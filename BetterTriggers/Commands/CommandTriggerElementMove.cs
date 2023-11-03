using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        Trigger trig;
        TriggerElement triggerElement;
        List<TriggerElement> OldParent;
        List<TriggerElement> NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandTriggerElementMove(Trigger trig, TriggerElement triggerElement, List<TriggerElement> NewParent, int NewInsertIndex)
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
            Project.CurrentProject.Triggers.RemoveInvalidReferences(trig, new List<TriggerElement>() { triggerElement });
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
