using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementMoveEx : ICommand
    {
        Project _project;
        string commandName = "Move Trigger Element";
        ExplorerElement explorerElement;
        ExplorerElement Parent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandExplorerElementMoveEx(Project project, ExplorerElement explorerElement, int NewInsertIndex)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.Parent = explorerElement.GetParent();
            this.OldInsertIndex = this.Parent.GetExplorerElements().IndexOf(explorerElement);
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, NewInsertIndex);
            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, NewInsertIndex);
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, OldInsertIndex);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
