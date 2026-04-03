using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyArray : ICommand
    {
        Project _project;
        string commandName = "Modify Variable Array";
        ExplorerElement explorerElement;
        Variable variable;
        bool isArray;
        RefCollection refCollection;

        public CommandVariableModifyArray(Project project, ExplorerElement explorerElement, Variable variable, bool isArray)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.variable = variable;
            this.isArray = isArray;
            this.refCollection = new RefCollection(project, explorerElement, variable);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            variable.IsArray = isArray;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());

            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            variable.IsArray = isArray;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            variable.IsArray = !isArray;
            refCollection.AddRefsToParent();
            _project.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
