using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        Components.TriggerEditor.TriggerElement triggerElement;
        TreeViewItem parent;
        int insertIndex = 0;

        public CommandTriggerElementCreate(Function function, TreeViewItem parent, int insertIndex)
        {
            this.triggerElement = new TriggerElement(function);
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            this.parent.Items.Insert(this.insertIndex, this.triggerElement);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            parent.Items.Insert(this.insertIndex, triggerElement);
        }

        public void Undo()
        {
            parent.Items.Remove(this.triggerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
