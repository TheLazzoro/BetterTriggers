using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.IO;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementRename : ICommand
    {
        string commandName = "Rename Explorer Element";
        ExplorerElement explorerElement;
        string oldFullPath;
        string newFullPath;
        RefCollection refCollection;


        public CommandExplorerElementRename(ExplorerElement explorerElement, string newFullPath)
        {
            this.oldFullPath = explorerElement.GetPath();
            this.newFullPath = newFullPath;
            this.explorerElement = explorerElement;
            this.refCollection = new RefCollection(explorerElement);
        }

        public void Execute()
        {
            Project.CurrentProject.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            Project.CurrentProject.EnableFileEvents(false);
            FileSystemUtil.RenameElementPath(explorerElement.GetPath(), newFullPath);
            Project.CurrentProject.EnableFileEvents(true);
            Project.CurrentProject.RecurseMoveElement(explorerElement, oldFullPath, newFullPath);

            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();
            explorerElement.IsSelected = true;
        }

        public void Undo()
        {
            Project.CurrentProject.EnableFileEvents(false);
            FileSystemUtil.RenameElementPath(explorerElement.GetPath(), oldFullPath);
            Project.CurrentProject.EnableFileEvents(true);
            Project.CurrentProject.RecurseMoveElement(explorerElement, newFullPath, oldFullPath);

            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            explorerElement.InvokeChange();
            refCollection.Notify();
            explorerElement.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
