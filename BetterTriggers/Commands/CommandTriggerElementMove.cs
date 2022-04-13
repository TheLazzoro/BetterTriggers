using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        TriggerElement triggerElement;
        List<TriggerElement> OldParent;
        List<TriggerElement> NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandTriggerElementMove(TriggerElement triggerElement, List<TriggerElement> NewParent, int NewInsertIndex)
        {
            this.triggerElement = triggerElement;
            this.OldParent = triggerElement.Parent;
            this.OldInsertIndex = this.OldParent.IndexOf(triggerElement);
            this.NewParent = NewParent;
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            triggerElement.ChangedPosition();
            CommandManager.AddCommand(this);
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
