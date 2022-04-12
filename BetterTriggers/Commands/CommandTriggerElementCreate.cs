using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        TriggerElement triggerElement;
        List<TriggerElement> parent;
        Function function;
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
            CommandManager.AddCommand(this);

            //triggerControl.OnStateChange();
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);

            //triggerControl.OnStateChange();
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();

            //triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
