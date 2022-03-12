using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using GUI.Components.TriggerEditor;
using BetterTriggers.Controllers;
using Model.SaveableData;
using GUI.Components;

namespace GUI.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        TriggerControl triggerControl;
        TriggerElement triggerElement;
        TreeViewItem parent;
        int treeIndex;
        Function functionToRemove;

        public CommandTriggerElementDelete(TriggerControl triggerControl, TriggerElement triggerElement, TreeViewItem parent)
        {
            this.triggerControl = triggerControl;
            this.triggerElement = triggerElement;
            this.parent = parent;

            this.treeIndex = parent.Items.IndexOf(triggerElement);
        }
        
        public void Execute()
        {
            if (parent is NodeEvent)
            {
                functionToRemove = triggerElement.explorerElementTrigger.trigger.Events[treeIndex];
                triggerElement.explorerElementTrigger.trigger.Events.Remove(functionToRemove);
            }
            if (parent is NodeCondition)
            {
                functionToRemove = triggerElement.explorerElementTrigger.trigger.Conditions[treeIndex];
                triggerElement.explorerElementTrigger.trigger.Conditions.Remove(functionToRemove);
            }
            if (parent is NodeAction)
            {
                functionToRemove = triggerElement.explorerElementTrigger.trigger.Actions[treeIndex];
                triggerElement.explorerElementTrigger.trigger.Actions.Remove(functionToRemove);
            }

            ControllerVariable controller = new ControllerVariable();
            controller.RecurseRemoveVariableRefs(triggerElement.function, triggerElement.explorerElementTrigger.trigger.Id); // Questionable
            // We are passing the TriggerElement's 'function' member and not the one found on 'explorerElementTrigger'.
            // We might need to refactor 'TriggerElement' so it listens to changes made on the 'explorerElementTrigger'.
            // Observer pattern is a possibility.

            parent.Items.Remove(this.triggerElement);
            CommandManager.AddCommand(this);

            triggerControl.OnStateChange();
        }

        public void Redo()
        {
            if (parent is NodeEvent)
                triggerElement.explorerElementTrigger.trigger.Events.Remove(functionToRemove);
            if (parent is NodeCondition)
                triggerElement.explorerElementTrigger.trigger.Conditions.Remove(functionToRemove);
            if (parent is NodeAction)
                triggerElement.explorerElementTrigger.trigger.Actions.Remove(functionToRemove);

            ControllerVariable controller = new ControllerVariable();
            controller.RecurseRemoveVariableRefs(triggerElement.function, triggerElement.explorerElementTrigger.trigger.Id);

            parent.Items.Remove(this.triggerElement);

            triggerControl.OnStateChange();
        }

        public void Undo()
        {
            if (parent is NodeEvent)
                triggerElement.explorerElementTrigger.trigger.Events.Insert(treeIndex, functionToRemove);
            if (parent is NodeCondition)
                triggerElement.explorerElementTrigger.trigger.Conditions.Insert(treeIndex, functionToRemove);
            if (parent is NodeAction)
                triggerElement.explorerElementTrigger.trigger.Actions.Insert(treeIndex, functionToRemove);

            ControllerVariable controller = new ControllerVariable();
            controller.RecurseAddVariableRefs(triggerElement.function, triggerElement.explorerElementTrigger.trigger.Id);

            parent.Items.Insert(this.treeIndex, triggerElement);
            triggerElement.IsSelected = true;

            triggerControl.OnStateChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
