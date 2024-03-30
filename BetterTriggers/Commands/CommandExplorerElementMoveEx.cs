using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandExplorerElementMoveEx : ICommand
    {
        string commandName = "Move Trigger Element";
        ExplorerElement explorerElement;
        ExplorerElement Parent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandExplorerElementMoveEx(ExplorerElement explorerElement, int NewInsertIndex)
        {
            this.explorerElement = explorerElement;
            this.Parent = explorerElement.GetParent();
            this.OldInsertIndex = this.Parent.GetExplorerElements().IndexOf(explorerElement);
            this.NewInsertIndex = NewInsertIndex;
        }

        public void Execute()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, NewInsertIndex);
            Project.CurrentProject.CommandManager.AddCommand(this);
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
