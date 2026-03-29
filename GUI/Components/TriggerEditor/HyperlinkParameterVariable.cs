using BetterTriggers.Commands;
using BetterTriggers.Containers;
using BetterTriggers.Models.EditorData;
using System.Windows;

namespace GUI.Components.TriggerEditor
{
    public class HyperlinkParameterVariable : HyperlinkBT
    {
        private Variable variable;
        private Project _project;

        public HyperlinkParameterVariable(Project project, Variable variable, Parameter parameter, string text)
            : base(parameter, text)
        {
            _project = project;
            this.variable = variable;

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParameterWindow(_project, parameter, variable.War3Type.Type);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandVariableModifyInitialValue command = new CommandVariableModifyInitialValue(_project, variable, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
