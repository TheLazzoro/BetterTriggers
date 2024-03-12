using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementReplace : ICommand
    {
        string commandName = "Replace Trigger Element";
        ExplorerElement explorerElement;
        TriggerElement toReplace;
        TriggerElement toInsert;
        TriggerElement parent;
        int insertIndex = 0;

        public CommandTriggerElementReplace(ExplorerElement explorerElement, TriggerElement toReplace, TriggerElement toInsert)
        {
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
            Project.CurrentProject.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            toReplace.RemoveFromParent();
            toInsert.SetParent(parent, insertIndex);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            toInsert.RemoveFromParent();
            toReplace.SetParent(parent, insertIndex);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
