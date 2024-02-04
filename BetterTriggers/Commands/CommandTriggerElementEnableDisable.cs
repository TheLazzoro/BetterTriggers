using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementEnableDisable : ICommand
    {
        string commandName = "Change Enable Trigger Element";
        ECA_Saveable triggerElement;

        public CommandTriggerElementEnableDisable(ECA_Saveable triggerElement)
        {
            this.triggerElement = triggerElement;
        }

        public void Execute()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
            triggerElement.ChangedEnabled();
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
            triggerElement.ChangedEnabled();
        }

        public void Undo()
        {
            triggerElement.isEnabled = !triggerElement.isEnabled;
            triggerElement.ChangedEnabled();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
