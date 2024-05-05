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
        TriggerElementCollection elementsToDelete;
        TriggerElement Parent;
        int insertIndex = 0;

        List<RefCollection> refCollections = new List<RefCollection>();

        public CommandTriggerElementDelete(ExplorerElement element, TriggerElementCollection elementsToDelete)
        {
            this.explorerElement = element;
            this.elementsToDelete = elementsToDelete;
            this.Parent = elementsToDelete.Elements[0].GetParent();
            this.insertIndex = this.Parent.IndexOf(elementsToDelete.Elements[0]);

            elementsToDelete.Elements.ForEach(el =>
            {
                if(el is LocalVariable localVar)
                {
                    RefCollection refCollection = new RefCollection(localVar.variable);
                    this.refCollections.Add(refCollection);
                }
            });

            if (elementsToDelete.Elements[0] is ParameterDefinition)
            {
                RefCollection refCollection = null;
                switch (element.ElementType)
                {
                    case ExplorerElementEnum.ActionDefinition:
                        refCollection = new RefCollection(element.actionDefinition);
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        refCollection = new RefCollection(element.conditionDefinition);
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        refCollection = new RefCollection(element.functionDefinition);
                        break;
                    default:
                        break;
                }
                refCollections.Add(refCollection);
            }
        }

        public void Execute()
        {
            for (int i = 0; i < elementsToDelete.Count(); i++)
            {
                elementsToDelete.Elements[i].RemoveFromParent();
            }
            foreach (var refCollection in refCollections)
            {
                refCollection.ResetParameters();
            }

            refCollections.ForEach(r => r.RemoveRefsFromParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);
            Project.CurrentProject.CommandManager.AddCommand(this);

            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            for (int i = 0; i < elementsToDelete.Count(); i++)
            {
                elementsToDelete.Elements[i].RemoveFromParent();
            }
            foreach (var refCollection in refCollections)
            {
                refCollection.ResetParameters();
            }

            refCollections.ForEach(r => r.RemoveRefsFromParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            for (int i = 0; i < elementsToDelete.Count(); i++)
            {
                elementsToDelete.Elements[i].SetParent(Parent, insertIndex + i);
            }
            foreach (var refCollection in refCollections)
            {
                refCollection.RevertToOldParameters();
            }

            refCollections.ForEach(r => r.AddRefsToParent());
            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
