using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCreate : ICommand
    {
        Project _project;
        string commandName = "Create Trigger Element";
        ExplorerElement _explorerElement;
        TriggerElement triggerElement;
        TriggerElement parent;
        int insertIndex = 0;
        RefCollection? refCollection;

        public CommandTriggerElementCreate(Project project, ExplorerElement explorerElement, TriggerElement triggerElement, TriggerElement parent, int insertIndex)
        {
            _project = project;
            _explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
            if (triggerElement is ParameterDefinition)
            {
                refCollection = new RefCollection(project, explorerElement);
            }
        }

        public void Execute()
        {
            triggerElement.SetParent(parent, insertIndex);
            _project.CommandManager.AddCommand(this);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            _explorerElement.InvokeChange();
            triggerElement.IsSelected = true;
        }

        public void Redo()
        {
            triggerElement.SetParent(parent, insertIndex);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            _explorerElement.InvokeChange();
            triggerElement.IsSelected = true;
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
