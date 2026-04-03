using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementReplace : ICommand
    {
        Project _project;
        string commandName = "Replace Trigger Element";
        ExplorerElement explorerElement;
        TriggerElement toReplace;
        TriggerElement toInsert;
        TriggerElement parent;
        int insertIndex = 0;

        public CommandTriggerElementReplace(Project project, ExplorerElement explorerElement, TriggerElement toReplace, TriggerElement toInsert)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.toReplace = toReplace;
            this.toInsert = toInsert;
            this.parent = toReplace.GetParent();
            this.insertIndex = parent.IndexOf(toReplace);
        }

        public void Execute()
        {
            toReplace.RemoveFromParent();
            toInsert.SetParent(parent, insertIndex);
            _project.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
            toInsert.IsSelected = true;
        }

        public void Redo()
        {
            toReplace.RemoveFromParent();
            toInsert.SetParent(parent, insertIndex);
            explorerElement.InvokeChange();
            toInsert.IsSelected = true;
        }

        public void Undo()
        {
            toInsert.RemoveFromParent();
            toReplace.SetParent(parent, insertIndex);
            explorerElement.InvokeChange();
            toReplace.IsSelected = true;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
