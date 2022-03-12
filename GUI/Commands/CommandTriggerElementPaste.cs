using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components;
using GUI.Components.TriggerEditor;
using Model.EditorData;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandTriggerElementPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        TriggerControl triggerControl;
        Components.TriggerEditor.TriggerElement triggerElement;
        TreeViewItem parent;
        int pastedIndex = 0;

        public CommandTriggerElementPaste(TriggerControl triggerControl, Function function, TreeViewItem parent, int pastedIndex)
        {
            this.triggerControl = triggerControl;
            this.triggerElement = new Components.TriggerEditor.TriggerElement(function, triggerControl);
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
