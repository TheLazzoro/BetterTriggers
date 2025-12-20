using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.Collections.Generic;
using System.Linq;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementMove : ICommand
    {
        string commandName = "Move Trigger Element";
        ExplorerElement explorerElement;
        List<TriggerElement> triggerElements;
        TriggerElement OldParent;
        TriggerElement NewParent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;
        RefCollection refCollection;

        public CommandTriggerElementMove(ExplorerElement explorerElement, List<TriggerElement> triggerElement, TriggerElementCollection NewParent, int NewInsertIndex)
        {
            this.explorerElement = explorerElement;
            this.triggerElements = triggerElement;
            this.OldParent = triggerElement.First().GetParent();
            this.OldInsertIndex = this.OldParent.IndexOf(triggerElement.First());
            this.NewParent = NewParent;
            this.NewInsertIndex = NewInsertIndex;
            if (triggerElement is ParameterDefinition)
            {
                refCollection = new RefCollection(explorerElement);
            }
        }

        public void Execute()
        {
            foreach (var triggerElement in triggerElements)
            {
                triggerElement.RemoveFromParent();
                triggerElement.SetParent(NewParent, NewInsertIndex);
            }
            TriggerValidator validator = new TriggerValidator(explorerElement);
            validator.RemoveInvalidReferences(NewParent);
            Project.CurrentProject.CommandManager.AddCommand(this);
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            explorerElement.InvokeChange();
            foreach (var triggerElement in triggerElements)
            {
                triggerElement.IsSelected_Multi = true;
            }
            triggerElements.Last().IsSelected = true;
        }

        public void Redo()
        {
            foreach (var triggerElement in triggerElements)
            {
                triggerElement.RemoveFromParent();
                triggerElement.SetParent(NewParent, NewInsertIndex);
                triggerElement.IsSelected_Multi = true;
            }
            if (refCollection != null)
            {
                refCollection.ResetParameters();
            }
            explorerElement.InvokeChange();
            foreach (var triggerElement in triggerElements)
            {
                triggerElement.IsSelected_Multi = true;
            }
            triggerElements.Last().IsSelected = true;
        }

        public void Undo()
        {
            foreach (var triggerElement in triggerElements.OrderByDescending(x => x.IndexInParent()))
            {
                triggerElement.RemoveFromParent();
                triggerElement.SetParent(OldParent, OldInsertIndex);
                triggerElement.IsSelected_Multi = true;
            }
            if (refCollection != null)
            {
                refCollection.RevertToOldParameters();
            }
            explorerElement.InvokeChange();
            foreach (var triggerElement in triggerElements)
            {
                triggerElement.IsSelected_Multi = true;
            }
            triggerElements.Last().IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
