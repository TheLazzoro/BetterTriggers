using System;
using System.Collections.Generic;
using System.IO;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementDelete : ICommand
    {
        string commandName = "Delete Explorer Element";
        IExplorerElement deletedElement;
        IExplorerElement parent;
        int index;
        RefCollection refCollection;

        public CommandExplorerElementDelete(IExplorerElement deletedElement)
        {
            this.deletedElement = deletedElement;
            this.parent = deletedElement.GetParent();
            this.index = parent.GetExplorerElements().IndexOf(deletedElement);
            this.refCollection = new RefCollection(deletedElement);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            deletedElement.RemoveFromParent();
            deletedElement.Deleted();

            if (deletedElement is ExplorerElementTrigger)
                Project.CurrentProject.References.RemoveReferrer(deletedElement as ExplorerElementTrigger);

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            deletedElement.RemoveFromParent();
            deletedElement.Deleted();

            Project.CurrentProject.EnableFileEvents(false);
            FileSystemUtil.Delete(deletedElement.GetPath());
            Project.CurrentProject.EnableFileEvents(true);

            
            
            if (deletedElement is ExplorerElementTrigger)
                Project.CurrentProject.References.RemoveReferrer(deletedElement as ExplorerElementTrigger);
        }

        public void Undo()
        {
            deletedElement.SetParent(parent, index);
            var project = Project.CurrentProject;
            project.EnableFileEvents(false);

            project.RecurseCreateElementsWithContent(deletedElement);
            project.AddElementToContainer(deletedElement);
            deletedElement.UpdateMetadata(); // this is important because we do a pseudo-undo (create the file from scratch)
            // We may want to do the same 

            project.EnableFileEvents(true);
            refCollection.AddRefsToParent();
            project.AllElements.Add(deletedElement.GetPath(), deletedElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
