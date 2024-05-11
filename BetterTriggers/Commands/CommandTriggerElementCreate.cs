using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        string commandName = "Create Trigger Element";
        ExplorerElement _explorerElement;
        TriggerElement triggerElement;
        TriggerElement parent;
        int insertIndex = 0;
        RefCollection? refCollection;

        public CommandTriggerElementCreate(ExplorerElement explorerElement, TriggerElement triggerElement, TriggerElement parent, int insertIndex)
        {
            _explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
            if (triggerElement is ParameterDefinition)
            {
                refCollection = new RefCollection(explorerElement);
            }
        }

        public void Execute()
        {
            var project = Project.CurrentProject;
            triggerElement.SetParent(parent, insertIndex);
            project.CommandManager.AddCommand(this);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            _explorerElement.InvokeChange();
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            _explorerElement.InvokeChange();
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            if (refCollection != null)
            {
                refCollection.RevertToOldParameters();
            }
            _explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
