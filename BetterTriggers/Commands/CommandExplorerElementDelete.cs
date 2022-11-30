using System;
using System.Collections.Generic;
using System.IO;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

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
                References.RemoveReferrer(deletedElement as ExplorerElementTrigger);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            deletedElement.RemoveFromParent();
            deletedElement.Deleted();
            ControllerProject controllerProject = new ControllerProject();
            
            controllerProject.SetEnableFileEvents(false);
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.DeleteElement(deletedElement.GetPath());
            controllerProject.SetEnableFileEvents(true);

            
            
            if (deletedElement is ExplorerElementTrigger)
                References.RemoveReferrer(deletedElement as ExplorerElementTrigger);
        }

        public void Undo()
        {
            deletedElement.SetParent(parent, index);
            deletedElement.Created(index);
            ControllerProject controller = new ControllerProject();
            controller.SetEnableFileEvents(false);

            controller.RecurseCreateElementsWithContent(deletedElement);
            controller.AddElementToContainer(deletedElement);
            deletedElement.UpdateMetadata(); // this is important because we do a pseudo-undo (create the file from scratch)
            // We may want to do the same 

            controller.SetEnableFileEvents(true);
            refCollection.AddRefsToParent();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
