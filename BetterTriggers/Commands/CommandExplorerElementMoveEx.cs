using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

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
            explorerElement.ChangedPosition();
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, NewInsertIndex);
            explorerElement.ChangedPosition();
        }

        public void Undo()
        {
            explorerElement.RemoveFromParent();
            explorerElement.SetParent(Parent, OldInsertIndex);
            explorerElement.ChangedPosition();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
