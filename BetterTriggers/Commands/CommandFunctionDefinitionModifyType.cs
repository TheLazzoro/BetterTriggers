using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Collections.Generic;

namespace BetterTriggers.Commands
{
    public class CommandFunctionDefinitionModifyType : ICommand
    {
        Project _project;
        string commandName = "Modify Function Definition Type";
        ExplorerElement explorerElement;
        FunctionDefinition functionDef;
        War3Type selectedType;
        War3Type previousType;
        RefCollection refCollection;

        List<ReturnStatement> returnStatements = new List<ReturnStatement>();
        List<Parameter> oldParameters = new List<Parameter>();
        List<Parameter> newParameters = new List<Parameter>();

        public CommandFunctionDefinitionModifyType(Project project, ExplorerElement explorerElement, FunctionDefinition functionDef, War3Type selectedType)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.functionDef = functionDef;
            this.selectedType = selectedType;
            this.previousType = functionDef.ReturnType.War3Type;
            this.refCollection = new RefCollection(project, explorerElement);

            // A way to reset all return statements
            var ecas = _project.GetTriggerElementsFromFunctionDefinition(functionDef);
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
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.RemoveRefsFromParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = newParameters[i];
            }
            _project.References.UpdateReferences(functionDef);
            _project.CommandManager.AddCommand(this);
            explorerElement.InvokeChange();
        }

        public void Redo()
        {
            functionDef.ReturnType.War3Type = selectedType;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.RemoveRefsFromParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = newParameters[i];
            }
            _project.References.UpdateReferences(functionDef);
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            functionDef.ReturnType.War3Type = previousType;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.AddRefsToParent();
            for (int i = 0; i < returnStatements.Count; i++)
            {
                var returnStatement = returnStatements[i];
                returnStatement.function.parameters[0] = oldParameters[i];
            }
            _project.References.UpdateReferences(functionDef);
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
