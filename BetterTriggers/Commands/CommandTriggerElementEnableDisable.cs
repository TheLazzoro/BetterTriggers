using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementEnableDisable : ICommand
    {
        Project _project;
        string commandName = "Change Enable Trigger Element";
        ExplorerElement _explorerElement;
        List<ECA> _ecas;

        public CommandTriggerElementEnableDisable(Project project, ExplorerElement explorerElement, List<ECA> ecas)
        {
            _project = project;
            _explorerElement = explorerElement;
            _ecas = ecas;
        }

        public void Execute()
        {
            foreach (var eca in _ecas)
            {
                eca.IsEnabled = !eca.IsEnabled;
            }
            _project.CommandManager.AddCommand(this);
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
