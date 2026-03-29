using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementCreate : ICommand
    {
        Project _project;
        string commandName = "Create Explorer Element";
        ExplorerElement parent;
        ExplorerElement createdElement;
        int insertIndex;

        public CommandExplorerElementCreate(Project project, ExplorerElement createdElement, ExplorerElement parent, int insertIndex)
        {
            _project = project;
            this.createdElement = createdElement;
            this.parent = parent;
            this.insertIndex = insertIndex;
        }

        public void Execute()
        {
            createdElement.SetParent(parent, insertIndex);
            createdElement.IsSelected = true;

            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            createdElement.SetParent(parent, insertIndex);

            _project.EnableFileEvents(false);
            _project.RecurseCreateElementsWithContent(createdElement);
            _project.EnableFileEvents(true);
            createdElement.IsSelected = true;
        }

        public void Undo()
        {
            createdElement.RemoveFromParent();

            _project.EnableFileEvents(false);
            FileSystemUtil.Delete(createdElement.GetPath());
            _project.EnableFileEvents(true);
            createdElement.InvokeDelete();
        }

        public string GetCommandName()
        {
            return commandName;
        }
        
    }
}

