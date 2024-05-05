
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCutPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        ExplorerElement from;
        ExplorerElement to;
        TriggerElement listToCut;
        TriggerElement listToPaste;
        TriggerElement cutParent;
        TriggerElement pasteParent;
        List<RefCollection> refCollections = new List<RefCollection>();

        public CommandTriggerElementCutPaste(ExplorerElement from, ExplorerElement to, TriggerElement listToPaste, TriggerElement pasteParent, int pastedIndex)
        {
            this.from = from;
            this.to = to;
            this.listToCut = CopiedElements.CutTriggerElements;
            this.cutParent = listToCut.Elements[0].GetParent();
            this.cutIndex = listToCut.Elements[0].GetParent().Elements.IndexOf(listToCut.Elements[0]);
            this.listToPaste = listToPaste;
            this.pasteParent = pasteParent;
            this.pastedIndex = pastedIndex;

            if (listToCut.Elements[0] is ParameterDefinition)
            {
                RefCollection refCollection = null;
                switch (from.ElementType)
                {
                    case ExplorerElementEnum.ActionDefinition:
                        refCollection = new RefCollection(from.actionDefinition);
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        refCollection = new RefCollection(from.conditionDefinition);
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        refCollection = new RefCollection(from.functionDefinition);
                        break;
                    default:
                        break;
                }
                refCollections.Add(refCollection);

                switch (to.ElementType)
                {
                    case ExplorerElementEnum.ActionDefinition:
                        refCollection = new RefCollection(to.actionDefinition);
                        break;
                    case ExplorerElementEnum.ConditionDefinition:
                        refCollection = new RefCollection(to.conditionDefinition);
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        refCollection = new RefCollection(to.functionDefinition);
                        break;
                    default:
                        break;
                }
                refCollections.Add(refCollection);
            }
        }

        public void Execute()
        {
            TriggerValidator validator = new TriggerValidator(to);
            validator.RemoveInvalidReferences(listToPaste);
            for (int i = 0; i < listToCut.Count(); i++)
            {
                listToCut.Elements[i].RemoveFromParent();
            }
            if (cutParent == pasteParent && pastedIndex > cutIndex)
            {
                this.pastedIndex = pastedIndex - listToPaste.Count();
            }

            for (int i = 0; i < listToPaste.Count(); i++)
            {
                var toPaste = listToPaste.Elements[i];
                toPaste.SetParent(pasteParent, pastedIndex + i);
                if (toPaste is ParameterDefinition paramDef)
                {
                    var paramParent = (ParameterDefinitionCollection)pasteParent;
                    paramDef.Name = paramParent.GenerateParameterDefName();
                }
            }

            foreach (var refCollection in refCollections)
            {
                refCollection.ResetParameters();
            }

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);
            CopiedElements.CutTriggerElements = null; // Reset
            Project.CurrentProject.CommandManager.AddCommand(this);

            from.InvokeChange();
            to.InvokeChange();
        }

        public void Redo()
        {
            for (int i = 0; i < listToCut.Count(); i++)
            {
                listToCut.Elements[i].RemoveFromParent();
            }
            for (int i = 0; i < listToPaste.Count(); i++)
            {
                listToPaste.Elements[i].SetParent(pasteParent, pastedIndex + i);
            }

            foreach (var refCollection in refCollections)
            {
                refCollection.ResetParameters();
            }

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);

            from.InvokeChange();
            to.InvokeChange();
        }

        public void Undo()
        {
            for (int i = 0; i < listToCut.Count(); i++)
            {
                listToCut.Elements[i].SetParent(cutParent, cutIndex + i);
            }
            for (int i = 0; i < listToPaste.Count(); i++)
            {
                listToPaste.Elements[i].RemoveFromParent();
            }
            foreach (var refCollection in refCollections)
            {
                refCollection.RevertToOldParameters();
            }

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);

            from.InvokeChange();
            to.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}