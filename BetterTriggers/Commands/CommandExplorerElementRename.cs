using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementRename : ICommand
    {
        string commandName = "Rename Explorer Element";
        IExplorerElement explorerElement;
        string oldFullPath;
        string newFullPath;
        RefCollection refCollection;


        public CommandExplorerElementRename(IExplorerElement explorerElement, string newFullPath)
        {
            this.oldFullPath = explorerElement.GetPath();
            this.newFullPath = newFullPath;
            this.explorerElement = explorerElement;
            this.refCollection = new RefCollection(explorerElement);
        }

        public void Execute()
        {
            ControllerProject controller = new ControllerProject();
            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            explorerElement.ChangedPosition();
            refCollection.Notify();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            ControllerProject controller = new ControllerProject();
            Project.EnableFileEvents(false);
            ControllerFileSystem.RenameElementPath(explorerElement.GetPath(), newFullPath);
            Project.EnableFileEvents(true);

            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            explorerElement.ChangedPosition();
            refCollection.Notify();
        }

        public void Undo()
        {
            ControllerProject controller = new ControllerProject();
            Project.EnableFileEvents(false);
            ControllerFileSystem.RenameElementPath(explorerElement.GetPath(), oldFullPath);
            Project.EnableFileEvents(true);

            controller.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);

            explorerElement.ChangedPosition();
            refCollection.Notify();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
