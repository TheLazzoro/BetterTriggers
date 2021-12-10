using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;

namespace GUI.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        TriggerElement triggerElement;
        TreeViewItem parent;
        int treeIndex;

        public CommandTriggerElementDelete(TriggerElement triggerElement, TreeViewItem parent)
        {
            this.triggerElement = triggerElement;
            this.parent = parent;

            this.treeIndex = parent.Items.IndexOf(triggerElement);
        }
        
        public void Execute()
        {
            parent.Items.Remove(this.triggerElement);
            
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            parent.Items.Remove(this.triggerElement);
        }

        public void Undo()
        {
            parent.Items.Insert(this.treeIndex, triggerElement);
            triggerElement.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
