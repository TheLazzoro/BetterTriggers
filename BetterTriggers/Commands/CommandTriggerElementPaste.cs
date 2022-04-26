
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Model.EditorData;
using Model.SaveableData;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        int pastedIndex = 0;
        List<TriggerElement> listToPaste;
        List<TriggerElement> parent;

        public CommandTriggerElementPaste(List<TriggerElement> listToPaste, List<TriggerElement> parent, int pastedIndex)
        {
            this.listToPaste = listToPaste;
            this.parent = parent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            for (int i = 0; i < listToPaste.Count; i++)
            {
                listToPaste[i].SetParent(parent, pastedIndex + i);
            }
            CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            for (int i = 0; i < listToPaste.Count; i++)
            {
                listToPaste[i].SetParent(parent, pastedIndex + i);
                listToPaste[i].Created(pastedIndex + i);
            }
        }

        public void Undo()
        {
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