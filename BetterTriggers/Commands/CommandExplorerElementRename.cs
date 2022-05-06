using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using Model.EditorData;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementRename : ICommand
    {
        string commandName = "Rename Explorer Element";
        IExplorerElement explorerElement;
        string oldFullPath;
        string newFullPath;

        public CommandExplorerElementRename(IExplorerElement explorerElement, string newFullPath)
        {
            this.oldFullPath = explorerElement.GetPath();
            this.newFullPath = newFullPath;

            this.explorerElement = explorerElement;
        }

        public void Execute()
        {
            ControllerProject controller = new ControllerProject();
            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            explorerElement.ChangedPosition();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            ControllerProject controller = new ControllerProject();
            controller.SetEnableFileEvents(false);
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.RenameElementPath(explorerElement.GetPath(), newFullPath);
            controller.SetEnableFileEvents(true);


            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            explorerElement.ChangedPosition();
        }

        public void Undo()
        {
            ControllerProject controller = new ControllerProject();
            controller.SetEnableFileEvents(false);
            ControllerFileSystem controllerFileSystem = new ControllerFileSystem();
            controllerFileSystem.RenameElementPath(explorerElement.GetPath(), oldFullPath);
            controller.SetEnableFileEvents(true);

            controller.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);

            explorerElement.ChangedPosition();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
