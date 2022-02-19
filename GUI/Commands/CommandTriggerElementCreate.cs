using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using Model.EditorData;
using Model.SaveableData;

namespace GUI.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        Components.TriggerEditor.TriggerElement triggerElement;
        Function function;
        TreeViewItem parent;
        int insertIndex = 0;

        public CommandTriggerElementCreate(ExplorerElementTrigger explorerElement, Function function, TreeViewItem parent, int insertIndex)
        {
            this.triggerElement = new TriggerElement(function, explorerElement);
            this.function = function;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            if (parent is NodeEvent)
                triggerElement.explorerElementTrigger.trigger.Events.Insert(insertIndex, function);
            if(parent is NodeCondition)
                triggerElement.explorerElementTrigger.trigger.Conditions.Insert(insertIndex, function);
            if (parent is NodeAction)
                triggerElement.explorerElementTrigger.trigger.Actions.Insert(insertIndex, function);

            this.parent.Items.Insert(this.insertIndex, this.triggerElement);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            if (parent is NodeEvent)
                triggerElement.explorerElementTrigger.trigger.Events.Insert(insertIndex, function);
            if (parent is NodeCondition)
                triggerElement.explorerElementTrigger.trigger.Conditions.Insert(insertIndex, function);
            if (parent is NodeAction)
                triggerElement.explorerElementTrigger.trigger.Actions.Insert(insertIndex, function);

            parent.Items.Insert(this.insertIndex, triggerElement);
        }

        public void Undo()
        {
            if (parent is NodeEvent)
                triggerElement.explorerElementTrigger.trigger.Events.Remove(function);
            if (parent is NodeCondition)
                triggerElement.explorerElementTrigger.trigger.Conditions.Remove(function);
            if (parent is NodeAction)
                triggerElement.explorerElementTrigger.trigger.Actions.Remove(function);

            parent.Items.Remove(this.triggerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
