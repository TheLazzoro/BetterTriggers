using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementDelete : ICommand
    {
        Project _project;
        string commandName = "Delete Explorer Element";
        ExplorerElement deletedElement;
        ExplorerElement parent;
        int index;
        RefCollection refCollection;

        public CommandExplorerElementDelete(Project project, ExplorerElement deletedElement)
        {
            _project = project;
            this.deletedElement = deletedElement;
            this.parent = deletedElement.GetParent();
            this.index = parent.GetExplorerElements().IndexOf(deletedElement);
            this.refCollection = new RefCollection(project, deletedElement);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            deletedElement.RemoveFromParent();
            deletedElement.RemoveFromUnsaved(true);

            if (deletedElement is ExplorerElement)
                _project.References.RemoveReferrer(deletedElement as ExplorerElement);

            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(t => t.InvokeChange());
            deletedElement.InvokeDelete();
            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            deletedElement.RemoveFromParent();
            deletedElement.RemoveFromUnsaved(true);

            _project.EnableFileEvents(false);
            FileSystemUtil.Delete(deletedElement.GetPath());
            _project.EnableFileEvents(true);

            if (deletedElement is ExplorerElement)
                _project.References.RemoveReferrer(deletedElement as ExplorerElement);

            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(t => t.InvokeChange());
            deletedElement.InvokeDelete();
        }

        public void Undo()
        {
            deletedElement.SetParent(parent, index);
            _project.EnableFileEvents(false);

            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(t => t.InvokeChange());
            _project.RecurseCreateElementsWithContent(deletedElement);
            _project.AddElementToContainer(deletedElement);
            deletedElement.UpdateMetadata(); // this is important because we do a pseudo-undo (create the file from scratch)
            // We may want to do the same 

            _project.EnableFileEvents(true);
            refCollection.AddRefsToParent();
            deletedElement.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
