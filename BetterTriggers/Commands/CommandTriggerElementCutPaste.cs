
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using BetterTriggers.Containers;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCutPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        int pastedIndex = 0;
        int cutIndex = 0;
        ExplorerElement from;
        ExplorerElement to;
        TriggerElementCollection listToCut;
        TriggerElementCollection listToPaste;
        TriggerElementCollection cutParent;
        TriggerElementCollection pasteParent;

        public CommandTriggerElementCutPaste(ExplorerElement from, ExplorerElement to, TriggerElementCollection listToPaste, TriggerElementCollection pasteParent, int pastedIndex)
        {
            this.from = from;
            this.to = to;
            this.listToCut = CopiedElements.CutTriggerElements;
            this.cutParent = listToCut.Elements[0].GetParent();
            this.cutIndex = listToCut.Elements[0].GetParent().Elements.IndexOf(listToCut.Elements[0]);
            this.listToPaste = listToPaste;
            this.pasteParent = pasteParent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            Project.CurrentProject.Triggers.RemoveInvalidReferences(to.trigger, listToPaste);
            for (int i = 0; i < listToCut.Count(); i++)
            {
                listToCut.Elements[i].RemoveFromParent();
            }
            if (cutParent == pasteParent && pastedIndex > cutIndex)
            {
                this.pastedIndex = pastedIndex - listToPaste.Count();
            }

            for (int i = 0; i < listToPaste.Count(); i++) {
                listToPaste.Elements[i].SetParent(pasteParent, pastedIndex + i);
            }

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);
            CopiedElements.CutTriggerElements = null; // Reset

            Project.CurrentProject.CommandManager.AddCommand(this);
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

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);
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

            Project.CurrentProject.References.UpdateReferences(from);
            Project.CurrentProject.References.UpdateReferences(to);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}