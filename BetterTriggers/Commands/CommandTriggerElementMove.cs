using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        Project _project;
        string commandName = "Move Trigger Element";
        ExplorerElement explorerElement;
        TriggerElement triggerElement;
        TriggerElement OldParent;
        TriggerElement NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;
        RefCollection refCollection;

        public CommandTriggerElementMove(Project project, ExplorerElement explorerElement, TriggerElement triggerElement, TriggerElementCollection NewParent, int NewInsertIndex)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.triggerElement = triggerElement;
            this.OldParent = triggerElement.GetParent();
            this.OldInsertIndex = this.OldParent.IndexOf(triggerElement);
            this.NewParent = NewParent;
            this.NewInsertIndex = NewInsertIndex;
            if (triggerElement is ParameterDefinition)
            {
                refCollection = new RefCollection(project, explorerElement);
            }
        }

        public void Execute()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            TriggerValidator validator = new TriggerValidator(_project, explorerElement);
            validator.RemoveInvalidReferences(NewParent);
            _project.CommandManager.AddCommand(this);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            explorerElement.InvokeChange();
            triggerElement.IsSelected = true;
        }

        public void Redo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(NewParent, NewInsertIndex);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            explorerElement.InvokeChange();
            triggerElement.IsSelected = true;
        }

        public void Undo()
        {
            triggerElement.RemoveFromParent();
            triggerElement.SetParent(OldParent, OldInsertIndex);
            if (refCollection != null)
            {
                refCollection.RevertToOldParameters();
            }
            explorerElement.InvokeChange();
            triggerElement.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
