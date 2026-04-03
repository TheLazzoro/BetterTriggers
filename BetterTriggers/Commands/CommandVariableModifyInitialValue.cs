using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;

namespace BetterTriggers.Commands
{
    public class CommandVariableModifyInitialValue : ICommand
    {
        Project _project;
        string commandName = "Modify Initial Value";
        Variable variable;
        Parameter newParameter;
        Parameter oldParameter;

        public CommandVariableModifyInitialValue(Project project, Variable variable, Parameter parameter)
        {
            _project = project;
            this.variable = variable;
            this.newParameter = parameter;
            this.oldParameter = variable.InitialValue;
        }

        public void Execute()
        {
            variable.InitialValue = newParameter;
            _project.CommandManager.AddCommand(this);
        }

        public void Redo()
        {
            variable.InitialValue = newParameter;
        }

        public void Undo()
        {
            variable.InitialValue = oldParameter;
        }

        public string GetCommandName()
        {
            return commandName;
        }
    }
}
