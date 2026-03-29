using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyDimension : ICommand
    {
        Project _project;
        string commandName = "Modify Variable Dimension";
        ExplorerElement explorerElement;
        Variable variable;
        bool isTwoDimensions;
        RefCollection refCollection;

        public CommandVariableModifyDimension(Project project, ExplorerElement explorerElement, Variable variable, bool isTwoDimensions)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.variable = variable;
            this.isTwoDimensions = isTwoDimensions;
            this.refCollection = new RefCollection(project, explorerElement, variable);
        }

        public void Execute()
        {
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            variable.IsTwoDimensions = isTwoDimensions;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());

            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            variable.IsTwoDimensions = isTwoDimensions;
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
        }

        public void Undo()
        {
            variable.IsTwoDimensions = !isTwoDimensions;
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
