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

            HandleChangedFileExtension(newFullPath);
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

            HandleChangedFileExtension(newFullPath);
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

            HandleChangedFileExtension(oldFullPath);
        }

        private void HandleChangedFileExtension(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            // Remove from container before changing the 'ElementType'
            Project.CurrentProject.RemoveElementFromContainer_WhenRenaming(explorerElement);
            Category category = Category.Get(TriggerCategory.TC_UNKNOWN);

            if (path.EndsWith(".j") || path.EndsWith(".lua"))
            {
                explorerElement.ElementType = ExplorerElementEnum.Script;
                category = Category.Get(TriggerCategory.TC_SCRIPT);
            }
            else if (path.EndsWith(".var"))
            {
                explorerElement.ElementType = ExplorerElementEnum.GlobalVariable;
                category = Category.Get(TriggerCategory.TC_SETVARIABLE);
            }
            else if (path.EndsWith(".trg"))
            {
                explorerElement.ElementType = ExplorerElementEnum.Trigger;
                category = Category.Get(TriggerCategory.TC_TRIGGER_NEW);
            }
            else if (path.EndsWith(".act"))
            {
                explorerElement.ElementType = ExplorerElementEnum.ActionDefinition;
                category = Category.Get(TriggerCategory.TC_ACTION_DEF);
            }
            else if (path.EndsWith(".cond"))
            {
                explorerElement.ElementType = ExplorerElementEnum.ConditionDefinition;
                category = Category.Get(TriggerCategory.TC_CONDITION_DEF);
            }
            else if (path.EndsWith(".func"))
            {
                explorerElement.ElementType = ExplorerElementEnum.FunctionDefinition;
                category = Category.Get(TriggerCategory.TC_FUNCTION_DEF);
            }
            else
            {
                explorerElement.ElementType = ExplorerElementEnum.None;
            }

            explorerElement.IconImage = category.Icon;
            Project.CurrentProject.AddElementToContainer(explorerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
