using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.Natives;

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
            if (parent is NodeEvent)
                this.triggerElement = new TriggerEvent(function);
            else if (parent is NodeCondition)
                this.triggerElement = new TriggerCondition(function);
            else if (parent is NodeAction)
                this.triggerElement = new Components.TriggerEditor.TriggerAction(function);

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
