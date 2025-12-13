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
        List<ECA> _ecas;

        public CommandTriggerElementEnableDisable(ExplorerElement explorerElement, List<ECA> ecas)
        {
            _explorerElement = explorerElement;
            _ecas = ecas;
        }

        public void Execute()
        {
            foreach (var eca in _ecas)
            {
                eca.IsEnabled = !eca.IsEnabled;
            }
            Project.CurrentProject.CommandManager.AddCommand(this);
            _explorerElement.InvokeChange();
        }

        public void Redo()
        {
            foreach (var eca in _ecas)
            {
                eca.IsEnabled = !eca.IsEnabled;
                eca.IsSelected = true;
            }
            _explorerElement.InvokeChange();
        }

        public void Undo()
        {
            foreach (var eca in _ecas)
            {
                eca.IsEnabled = !eca.IsEnabled;
                eca.IsSelected = true;
            }
            _explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
