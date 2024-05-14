using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandParameterDefinitionModifyType : ICommand
    {
        string commandName = "Modify Parameter Definition Type";
        ExplorerElement explorerElement;
        ParameterDefinition parameterDef;
        War3Type selectedType;
        War3Type previousType;
        RefCollection refCollection1;
        RefCollection refCollection2;

        public CommandParameterDefinitionModifyType(ExplorerElement explorerElement, ParameterDefinition parameterDef, War3Type selectedType)
        {
            this.explorerElement = explorerElement;
            this.parameterDef = parameterDef;
            this.selectedType = selectedType;
            this.previousType = parameterDef.ReturnType;

            refCollection1 = new RefCollection(explorerElement);
            refCollection2 = new RefCollection(parameterDef);
        }

        public void Execute()
        {
            parameterDef.ReturnType = selectedType;
            refCollection1.ResetParameters();
            refCollection2.ResetParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            Project.CurrentProject.CommandManager.AddCommand(this);
            refCollection1.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection1.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            parameterDef.ReturnType = selectedType;
            refCollection1.ResetParameters();
            refCollection2.ResetParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection1.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection1.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            parameterDef.ReturnType = previousType;
            refCollection1.RevertToOldParameters();
            refCollection2.RevertToOldParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection1.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection1.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
