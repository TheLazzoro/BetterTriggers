using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementEnableDisable : ICommand
    {
        string commandName = "Change Enable Trigger Element";
        ExplorerElement _explorerElement;
        ECA _eca;

        public CommandTriggerElementEnableDisable(ExplorerElement explorerElement, ECA eca)
        {
            _explorerElement = explorerElement;
            _eca = eca;
        }

        public void Execute()
        {
            _eca.isEnabled = !_eca.isEnabled;
            Project.CurrentProject.CommandManager.AddCommand(this);
            _explorerElement.InvokeChange();
        }

        public void Redo()
        {
            _eca.isEnabled = !_eca.isEnabled;
            _explorerElement.InvokeChange();
        }

        public void Undo()
        {
            _eca.isEnabled = !_eca.isEnabled;
            _explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
