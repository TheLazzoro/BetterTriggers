using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
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
            var project = Project.CurrentProject;
            var rootNode = project.projectFiles[0];
            newParent = project.FindExplorerElementFolder(rootNode, Path.GetDirectoryName(newFullPath));
            if(newParent == null)
            {
                newParent = Project.CurrentProject.GetRoot();
            }
            this.oldFullPath = explorerElement.GetPath();
            this.newFullPath = newFullPath;

            this.explorerElement = explorerElement;
            this.oldParent = explorerElement.GetParent();
            this.OldInsertIndex = this.oldParent.GetExplorerElements().IndexOf(explorerElement);
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            var project = Project.CurrentProject;
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);
            project.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            explorerElement.ChangedPosition(oldFullPath, newFullPath);

            project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);

            var project = Project.CurrentProject;
            project.EnableFileEvents(false);
            FileSystemUtil.Move(explorerElement.GetPath(), newParent.GetPath(), NewInsertIndex);
            project.EnableFileEvents(true);


            project.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            explorerElement.ChangedPosition(oldFullPath, newFullPath);
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(oldParent, OldInsertIndex);

            var project = Project.CurrentProject;
            project.EnableFileEvents(false);
            FileSystemUtil.Move(explorerElement.GetPath(), oldParent.GetPath(), OldInsertIndex);
            project.EnableFileEvents(true);

            project.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);

            explorerElement.ChangedPosition(newFullPath, oldFullPath);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
