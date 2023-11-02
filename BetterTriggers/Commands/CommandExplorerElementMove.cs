using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementMove : ICommand
    {
        string commandName = "Move Explorer Element";
        IExplorerElement explorerElement;
        IExplorerElement oldParent;
        IExplorerElement newParent;
        string oldFullPath;
        string newFullPath;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandExplorerElementMove(IExplorerElement explorerElement, string newFullPath, int NewInsertIndex)
        {
            ControllerProject controller = new ControllerProject();
            var rootNode = Project.projectFiles[0];
            newParent = controller.FindExplorerElementFolder(rootNode, Path.GetDirectoryName(newFullPath));
            this.oldFullPath = explorerElement.GetPath();
            this.newFullPath = newFullPath;

            this.explorerElement = explorerElement;
            this.oldParent = explorerElement.GetParent();
            this.OldInsertIndex = this.oldParent.GetExplorerElements().IndexOf(explorerElement);
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);
            ControllerProject controller = new ControllerProject();
            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            explorerElement.ChangedPosition();

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);

            ControllerProject controller = new ControllerProject();
            Project.EnableFileEvents(false);
            ControllerFileSystem.Move(explorerElement.GetPath(), newParent.GetPath(), NewInsertIndex);
            Project.EnableFileEvents(true);


            controller.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            explorerElement.ChangedPosition();
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(oldParent, OldInsertIndex);

            ControllerProject controller = new ControllerProject();
            Project.EnableFileEvents(false);
            ControllerFileSystem.Move(explorerElement.GetPath(), oldParent.GetPath(), OldInsertIndex);
            Project.EnableFileEvents(true);

            controller.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);

            explorerElement.ChangedPosition();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
