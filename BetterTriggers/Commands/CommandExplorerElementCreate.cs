using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementCreate : ICommand
    {
        string commandName = "Create Explorer Element";
        ExplorerElement parent;
        ExplorerElement createdElement;
        int insertIndex;

        public CommandExplorerElementCreate(ExplorerElement createdElement, ExplorerElement parent, int insertIndex)
        {
            this.createdElement = createdElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            createdElement.SetParent(parent, insertIndex);

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            createdElement.SetParent(parent, insertIndex);

            var project = Project.CurrentProject;
            project.EnableFileEvents(false);
            project.RecurseCreateElementsWithContent(createdElement);
            project.EnableFileEvents(true);
        }

        public void Undo()
        {
            createdElement.RemoveFromParent();

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

