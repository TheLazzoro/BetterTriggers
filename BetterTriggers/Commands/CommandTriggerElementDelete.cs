using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        TriggerElement triggerElement;
        List<TriggerElement> Parent;
        int insertIndex = 0;

        public CommandTriggerElementDelete(TriggerElement triggerElement)
        {
            this.triggerElement = triggerElement;
            this.Parent = triggerElement.Parent;
            this.insertIndex = this.Parent.IndexOf(triggerElement);
        }

        public void Execute()
        {
            triggerElement.RemoveFromParent();
            triggerElement.Deleted();
            CommandManager.AddCommand(this);

            //triggerControl.OnStateChange();
        }

        public void Redo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.Deleted();

            //triggerControl.OnStateChange();
        }

        public void Undo()
        {
            triggerElement.SetParent(Parent, insertIndex);
            triggerElement.Created();

            //triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
