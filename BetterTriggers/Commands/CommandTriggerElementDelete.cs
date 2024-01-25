using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementDelete : ICommand
    {
        string commandName = "Delete Trigger Element";
        ExplorerElement explorerElement;
        List<TriggerElement> elementsToDelete;
        List<TriggerElement> Parent;
        int insertIndex = 0;

        List<RefCollection> refCollections = new List<RefCollection>();

        public CommandTriggerElementDelete(ExplorerElement element, List<TriggerElement> elementsToDelete)
        {
            this.explorerElement = element;
            this.elementsToDelete = elementsToDelete;
            this.Parent = elementsToDelete[0].GetParent();
            this.insertIndex = this.Parent.IndexOf(elementsToDelete[0]);

            elementsToDelete.ForEach(el =>
            {
                if(el is LocalVariable localVar)
                {
                    RefCollection refCollection = new RefCollection(localVar.variable);
                    this.refCollections.Add(refCollection);
                }
            });
        }

        public void Execute()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();
            }

            refCollections.ForEach(r => r.RemoveRefsFromParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);

            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].RemoveFromParent();
                elementsToDelete[i].Deleted();
            }

            refCollections.ForEach(r => r.RemoveRefsFromParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);
        }

        public void Undo()
        {
            for (int i = 0; i < elementsToDelete.Count; i++)
            {
                elementsToDelete[i].SetParent(Parent, insertIndex + i);
                elementsToDelete[i].Created(insertIndex + i);
            }

            refCollections.ForEach(r => r.AddRefsToParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
