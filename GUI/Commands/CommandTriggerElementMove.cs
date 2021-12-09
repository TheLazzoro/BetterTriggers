using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;

namespace GUI.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        TreeViewItem triggerElement;
        TreeViewItem oldParent;
        TreeViewItem newParent;
        int oldIndex;
        int newIndex;

        public CommandTriggerElementMove(TreeViewItem triggerElement, TreeViewItem oldParent, TreeViewItem newParent, int newIndex)
        {
            this.triggerElement = triggerElement;
            this.oldParent = oldParent;
            this.newParent = newParent;

            this.oldIndex = oldParent.Items.IndexOf(triggerElement);
            this.newIndex = newIndex;
        }
        
        public void Execute()
        {
            oldParent.Items.Remove(this.triggerElement);
            newParent.Items.Insert(this.newIndex, triggerElement);
            
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            oldParent.Items.Remove(this.triggerElement);
            newParent.Items.Insert(this.newIndex, triggerElement);
        }

        public void Undo()
        {
            newParent.Items.Remove(this.triggerElement);
            oldParent.Items.Insert(this.oldIndex, triggerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
