using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        List<TriggerElement> elementsToDelete;
        List<TriggerElement> Parent;
        int insertIndex = 0;

        public CommandTriggerElementDelete(List<TriggerElement> elementsToDelete)
        {
            this.elementsToDelete = elementsToDelete;
            this.Parent = elementsToDelete[0].Parent;
            this.insertIndex = this.Parent.IndexOf(elementsToDelete[0]);
        }

        public void Execute()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();

            }
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();
            }
        }

        public void Undo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].SetParent(Parent, insertIndex + i);
                elementsToDelete[i].Created(insertIndex + i);
            }
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
