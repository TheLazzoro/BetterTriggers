using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyType : ICommand
    {
        Project _project;
        string commandName = "Modify Variable Type";
        ExplorerElement explorerElement;
        Variable variable;
        War3Type selectedType;
        War3Type previousType;
        Parameter newInitialValue;
        Parameter previousInitialValue;
        RefCollection refCollection;

        public CommandVariableModifyType(Project project, ExplorerElement explorerElement, Variable variable, War3Type selectedType)
        {
            _project = project;
            this.explorerElement = explorerElement;
            this.variable = variable;
            this.selectedType = selectedType;
            this.previousType = variable.War3Type;
            this.previousInitialValue = variable.InitialValue;
            this.newInitialValue = new Parameter();
            this.refCollection = new RefCollection(project, variable, selectedType, explorerElement);
        }

        public void Execute()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();

            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = newInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = selectedType;
            refCollection.RemoveRefsFromParent();
            _project.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public void Undo()
        {
            variable.SuppressChangedEvent = true;
            variable.InitialValue = previousInitialValue;
            variable.SuppressChangedEvent = false;
            variable.War3Type = previousType;
            refCollection.AddRefsToParent();
            _project.References.UpdateReferences(variable);
            refCollection.TriggersToUpdate.ForEach(t => t.ShouldRefreshUIElements = true);
            refCollection.TriggersToUpdate.ForEach(el => el.InvokeChange());
            explorerElement.InvokeChange();
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
