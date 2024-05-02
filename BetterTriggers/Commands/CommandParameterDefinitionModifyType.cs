using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandParameterDefinitionModifyType : ICommand
    {
        string commandName = "Modify Parameter Definition Type";
        ParameterDefinition parameterDef;
        War3Type selectedType;
        War3Type previousType;
        RefCollection refCollection;

        public CommandParameterDefinitionModifyType(ParameterDefinition parameterDef, War3Type selectedType)
        {
            this.parameterDef = parameterDef;
            this.selectedType = selectedType;
            this.previousType = parameterDef.ReturnType;
            this.refCollection = new RefCollection(parameterDef);
        }

        public void Execute()
        {
            parameterDef.ReturnType = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            Project.CurrentProject.CommandManager.AddCommand(this);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Redo()
        {
            parameterDef.ReturnType = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            parameterDef.ReturnType = previousType;
            refCollection.AddRefsToParent();
            Project.CurrentProject.References.UpdateReferences(parameterDef);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
