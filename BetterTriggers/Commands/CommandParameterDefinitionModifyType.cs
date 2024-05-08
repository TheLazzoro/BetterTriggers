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
        RefCollection refCollection;

        public CommandParameterDefinitionModifyType(ExplorerElement explorerElement, ParameterDefinition parameterDef, War3Type selectedType)
        {
            this.explorerElement = explorerElement;
            this.parameterDef = parameterDef;
            this.selectedType = selectedType;
            this.previousType = parameterDef.ReturnType;

            switch (explorerElement.ElementType)
            {
                case ExplorerElementEnum.ActionDefinition:
                    refCollection = new RefCollection(explorerElement.actionDefinition);
                    break;
                case ExplorerElementEnum.ConditionDefinition:
                    refCollection = new RefCollection(explorerElement.conditionDefinition);
                    break;
                case ExplorerElementEnum.FunctionDefinition:
                    refCollection = new RefCollection(explorerElement.functionDefinition);
                    break;
                default:
                    break;
            }
        }

        public void Execute()
        {
            parameterDef.ReturnType = selectedType;
            refCollection.ResetParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            Project.CurrentProject.CommandManager.AddCommand(this);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Redo()
        {
            parameterDef.ReturnType = selectedType;
            refCollection.ResetParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            parameterDef.ReturnType = previousType;
            refCollection.RevertToOldParameters();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
