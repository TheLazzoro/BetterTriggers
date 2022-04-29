
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCutPaste : ICommand
    {
        string commandName = "Cut/Paste Trigger Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        List<TriggerElement> listToCut;
        List<TriggerElement> listToPaste;
        List<TriggerElement> cutParent;
        List<TriggerElement> pasteParent;

        public CommandTriggerElementCutPaste(List<TriggerElement> listToPaste, List<TriggerElement> pasteParent, int pastedIndex)
        {
            this.listToCut = ContainerCopiedElements.CutTriggerElements;
            this.cutParent = listToCut[0].Parent;
            this.cutIndex = listToCut[0].Parent.IndexOf(listToCut[0]);
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
            for (int i = 0; i < listToPaste.Count; i++)
            {
                listToPaste[i].SetParent(pasteParent, pastedIndex + i);
            }
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
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}