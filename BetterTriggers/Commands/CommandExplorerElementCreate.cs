using BetterTriggers.Containers;
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
            Project.EnableFileEvents(false);
            controllerProject.RecurseCreateElementsWithContent(createdElement);
            Project.EnableFileEvents(true);
        }

        public void Undo()
        {
            createdElement.RemoveFromParent();
            createdElement.Deleted();

            Project.EnableFileEvents(false);
            ControllerFileSystem.Delete(createdElement.GetPath());
            Project.EnableFileEvents(true);
        }

        public string GetCommandName()
        {
            return commandName;
        }
        
    }
}

