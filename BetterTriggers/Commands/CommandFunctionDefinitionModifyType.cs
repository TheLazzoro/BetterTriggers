using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.EditorData.TriggerEditor;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandFunctionDefinitionModifyType : ICommand
    {
        string commandName = "Modify Function Definition Type";
        ExplorerElement explorerElement;
        FunctionDefinition functionDef;
        War3Type selectedType;
        War3Type previousType;
        RefCollection refCollection;

        List<ReturnStatement> returnStatements = new List<ReturnStatement>();
        List<Parameter> oldParameters = new List<Parameter>();
        List<Parameter> newParameters = new List<Parameter>();

        public CommandFunctionDefinitionModifyType(ExplorerElement explorerElement, FunctionDefinition functionDef, War3Type selectedType)
        {
            this.explorerElement = explorerElement;
            this.functionDef = functionDef;
            this.selectedType = selectedType;
            this.previousType = functionDef.ReturnType.War3Type;
            this.refCollection = new RefCollection(functionDef);

            // A way to reset all return statements
            var ecas = Project.CurrentProject.GetTriggerElementsFromFunctionDefinition(functionDef);
            for (int i = 0; i < ecas.Count; i++)
            {
                var eca = ecas[i];
                if(eca is ReturnStatement returnStatement)
                {
                    returnStatements.Add(returnStatement);
                    oldParameters.Add(returnStatement.function.parameters[0]);
                    newParameters.Add(new Parameter());
                }
            }
        }

        public void Execute()
        {
            functionDef.ReturnType.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = newParameters[i];
            }
            Project.CurrentProject.References.UpdateReferences(functionDef);
            Project.CurrentProject.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            functionDef.ReturnType.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = newParameters[i];
            }
            Project.CurrentProject.References.UpdateReferences(functionDef);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            functionDef.ReturnType.War3Type = previousType;
            refCollection.AddRefsToParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = oldParameters[i];
            }
            Project.CurrentProject.References.UpdateReferences(functionDef);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
