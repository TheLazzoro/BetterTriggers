
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Controllers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCutPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        ExplorerElementTrigger from;
        ExplorerElementTrigger to;
        List<TriggerElement> listToCut;
        List<TriggerElement> listToPaste;
        List<TriggerElement> cutParent;
        List<TriggerElement> pasteParent;

        public CommandTriggerElementCutPaste(ExplorerElementTrigger from, ExplorerElementTrigger to, List<TriggerElement> listToPaste, List<TriggerElement> pasteParent, int pastedIndex)
        {
            this.from = from;
            this.to = to;
            this.listToCut = CopiedElements.CutTriggerElements;
            this.cutParent = listToCut[0].GetParent();
            this.cutIndex = listToCut[0].GetParent().IndexOf(listToCut[0]);
            this.listToPaste = listToPaste;
            this.pasteParent = pasteParent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            for (int i = 0; i < listToCut.Count; i++)
            {
                listToCut[i].RemoveFromParent();
                listToCut[i].Deleted();
            }
            if (cutParent == pasteParent && pastedIndex > cutIndex)
            {
                this.pastedIndex = pastedIndex - listToPaste.Count;
            }

            for (int i = 0; i < listToPaste.Count; i++) {
                listToPaste[i].SetParent(pasteParent, pastedIndex + i);
            }

            References.UpdateReferences(from);
            References.UpdateReferences(to);
            CopiedElements.CutTriggerElements = null; // Reset

            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            for (int i = 0; i < listToCut.Count; i++)
            {
                listToCut[i].RemoveFromParent();
                listToCut[i].Deleted();
            }
            for (int i = 0; i < listToPaste.Count; i++)
            {
                listToPaste[i].SetParent(pasteParent, pastedIndex + i);
                listToPaste[i].Created(pastedIndex + i);
            }

            References.UpdateReferences(from);
            References.UpdateReferences(to);
        }

        public void Undo()
        {
            for (int i = 0; i < listToCut.Count; i++)
            {
                listToCut[i].SetParent(cutParent, cutIndex + i);
                listToCut[i].Created(cutIndex + i);
            }
            for (int i = 0; i < listToPaste.Count; i++)
            {
                listToPaste[i].RemoveFromParent();
                listToPaste[i].Deleted();
            }

            References.UpdateReferences(from);
            References.UpdateReferences(to);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}