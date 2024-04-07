using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandFunctionDefinitionModifyType : ICommand
    {
        string commandName = "Modify Function Definition Type";
        FunctionDefinition functionDef;
        War3Type selectedType;
        War3Type previousType;
        RefCollection refCollection;

        public CommandFunctionDefinitionModifyType(FunctionDefinition functionDef, War3Type selectedType)
        {
            this.functionDef = functionDef;
            this.selectedType = selectedType;
            this.previousType = functionDef.ReturnType.War3Type;
            this.refCollection = new RefCollection(functionDef);
        }

        public void Execute()
        {
            functionDef.ReturnType.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(functionDef);
            Project.CurrentProject.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            functionDef.ReturnType.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            Project.CurrentProject.References.UpdateReferences(functionDef);
        }

        public void Undo()
        {
            functionDef.ReturnType.War3Type = previousType;
            refCollection.AddRefsToParent();
            Project.CurrentProject.References.UpdateReferences(functionDef);
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
