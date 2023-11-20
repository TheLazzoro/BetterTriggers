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
        IExplorerElement explorerElement;
        IExplorerElement Parent;
        int OldInsertIndex = 0;
        int NewInsertIndex = 0;

        public CommandExplorerElementMoveEx(IExplorerElement explorerElement, int NewInsertIndex)
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
            explorerElement.ChangedPosition(explorerElement.GetPath(), explorerElement.GetPath());
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, NewInsertIndex);
            explorerElement.ChangedPosition(explorerElement.GetPath(), explorerElement.GetPath());
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, OldInsertIndex);
            explorerElement.ChangedPosition(explorerElement.GetPath(), explorerElement.GetPath());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
