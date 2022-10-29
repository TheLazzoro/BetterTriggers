using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        ExplorerElementTrigger explorerElement;
        List<ITriggerElement> elementsToDelete;
        List<ITriggerElement> Parent;
        int insertIndex = 0;

        public CommandTriggerElementDelete(ExplorerElementTrigger element, List<ITriggerElement> elementsToDelete)
        {
            this.explorerElement = element;
            this.elementsToDelete = elementsToDelete;
            this.Parent = elementsToDelete[0].GetParent();
            this.insertIndex = this.Parent.IndexOf(elementsToDelete[0]);
        }

        public void Execute()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();
            }

            ControllerReferences controller = new ControllerReferences();
            controller.UpdateReferences(explorerElement);

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();
            }

            ControllerReferences controller = new ControllerReferences();
            controller.UpdateReferences(explorerElement);
        }

        public void Undo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].SetParent(Parent, insertIndex + i);
                elementsToDelete[i].Created(insertIndex + i);
            }

            ControllerReferences controller = new ControllerReferences();
            controller.UpdateReferences(explorerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
