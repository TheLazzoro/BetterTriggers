using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Utility;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandTriggerElementCutPaste : ICommand
    {
        Project _project;
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

        public CommandTriggerElementCutPaste(Project project, ExplorerElement from, ExplorerElement to, TriggerElement listToPaste, TriggerElement pasteParent, int pastedIndex)
        {
            _project = project;
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
                refCollections.Add(new RefCollection(project, from));
                refCollections.Add(new RefCollection(project, to));
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

            _project.References.UpdateReferences(from);
            _project.References.UpdateReferences(to);
            CopiedElements.CutTriggerElements = null; // Reset
            _project.CommandManager.AddCommand(this);

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

            _project.References.UpdateReferences(from);
            _project.References.UpdateReferences(to);

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

            _project.References.UpdateReferences(from);
            _project.References.UpdateReferences(to);

            from.InvokeChange();
            to.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}