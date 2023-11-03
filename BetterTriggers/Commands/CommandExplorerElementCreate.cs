using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
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

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            createdElement.SetParent(parent, insertIndex);
            createdElement.Created(insertIndex);

            var project = Project.CurrentProject;
            project.EnableFileEvents(false);
            project.RecurseCreateElementsWithContent(createdElement);
            project.EnableFileEvents(true);
        }

        public void Undo()
        {
            createdElement.RemoveFromParent();
            createdElement.Deleted();

            Project.CurrentProject.EnableFileEvents(false);
            FileSystemUtil.Delete(createdElement.GetPath());
            Project.CurrentProject.EnableFileEvents(true);
        }

        public string GetCommandName()
        {
            return commandName;
        }
        
    }
}

