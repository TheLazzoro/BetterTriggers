using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementMove : ICommand
    {
        Project _project;
        string commandName = "Move Explorer Element";
        ExplorerElement explorerElement;
        ExplorerElement oldParent;
        ExplorerElement newParent;
        string oldFullPath;
        string newFullPath;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandExplorerElementMove(Project project, ExplorerElement explorerElement, string newFullPath, int NewInsertIndex)
        {
            _project = project;
            var rootNode = project.projectFiles[0];
            newParent = project.FindExplorerElementFolder(rootNode, Path.GetDirectoryName(newFullPath));
            if(newParent == null)
            {
                newParent = _project.GetRoot();
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
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);
            _project.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            _project.CommandManager.AddCommand(this);
            explorerElement.IsSelected = true;
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(newParent, NewInsertIndex);

            _project.EnableFileEvents(false);
            FileSystemUtil.Move(explorerElement.GetPath(), newParent.GetPath(), NewInsertIndex);
            _project.EnableFileEvents(true);


            _project.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            explorerElement.IsSelected = true;
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(oldParent, OldInsertIndex);

            _project.EnableFileEvents(false);
            FileSystemUtil.Move(explorerElement.GetPath(), oldParent.GetPath(), OldInsertIndex);
            _project.EnableFileEvents(true);

            _project.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);
            explorerElement.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
