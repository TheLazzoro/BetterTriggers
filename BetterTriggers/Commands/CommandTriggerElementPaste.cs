using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        TriggerControl triggerControl;
        Components.TriggerEditor.TreeViewTriggerElement triggerElement;
        TreeViewItem parent;
        int pastedIndex = 0;

        public CommandTriggerElementPaste(TriggerControl triggerControl, Function function, TreeViewItem parent, int pastedIndex)
        {
            this.triggerControl = triggerControl;
            this.triggerElement = new Components.TriggerEditor.TreeViewTriggerElement(function, triggerControl);
            this.parent = parent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            parent.Items.Insert(this.pastedIndex, triggerElement);

            CommandManager.AddCommand(this);

            triggerControl.OnStateChange();
        }

        public void Redo()
        {
            parent.Items.Insert(this.pastedIndex, triggerElement);
            triggerControl.OnStateChange();
        }

        public void Undo()
        {
            parent.Items.Remove(this.triggerElement);
            triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
