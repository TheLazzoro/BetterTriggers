
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using BetterTriggers.Utility;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementPaste : ICommand
    {
        string commandName = "Paste Trigger Element";
        int pastedIndex = 0;
        ExplorerElement explorerElement;
        TriggerElement listToPaste;
        TriggerElement parent;

        public CommandTriggerElementPaste(ExplorerElement element, TriggerElementCollection listToPaste, TriggerElement parent, int pastedIndex)
        {
            this.explorerElement = element;
            this.listToPaste = listToPaste;
            this.parent = parent;
            this.pastedIndex = pastedIndex;
        }

        public void Execute()
        {
            TriggerValidator validator = new TriggerValidator(explorerElement);
            validator.RemoveInvalidReferences(listToPaste);
            for (int i = 0; i < listToPaste.Count(); i++)
            {
                var toPaste = listToPaste.Elements[i];
                toPaste.SetParent(parent, pastedIndex + i);
                if(toPaste is ParameterDefinition paramDef)
                {
                    var paramParent = (ParameterDefinitionCollection)parent;
                    paramDef.Name = paramParent.GenerateParameterDefName();
                }
            }

            Project.CurrentProject.References.UpdateReferences(explorerElement);
            Project.CurrentProject.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            for (int i = 0; i < listToPaste.Count(); i++)
            {
                listToPaste.Elements[i].SetParent(parent, pastedIndex + i);
            }


            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            for (int i = 0; i < listToPaste.Count(); i++)
            {
                listToPaste.Elements[i].RemoveFromParent();
            }

            Project.CurrentProject.References.UpdateReferences(explorerElement);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}