using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementCreate : ICommand
    {
        string commandName = "Create Explorer Element";
        IExplorerElement parent;
        IExplorerElement createdElement;
        int insertIndex;

        public CommandExplorerElementCreate(IExplorerElement createdElement, IExplorerElement parent, int insertIndex)
        {
            this.createdElement = createdElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            createdElement.SetParent(parent, insertIndex);
            createdElement.Created(insertIndex);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            createdElement.SetParent(parent, insertIndex);
            createdElement.Created(insertIndex);

            ControllerProject controllerProject = new ControllerProject();
            controllerProject.SetEnableFileEvents(false);
            controllerProject.RecurseCreateElementsWithContent(createdElement);
            controllerProject.SetEnableFileEvents(true);
        }

        public void Undo()
        {
            createdElement.RemoveFromParent();
            createdElement.Deleted();

            ControllerProject controllerProject = new ControllerProject();
            controllerProject.SetEnableFileEvents(false);
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.DeleteElement(createdElement.GetPath());
            controllerProject.SetEnableFileEvents(true);
        }

        public string GetCommandName()
        {
            return commandName;
        }
        
    }
}

