using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GUI.Components.TriggerEditor
{
    public class HyperlinkParameterVariable : HyperlinkBT
    {
        private Variable variable;

        public HyperlinkParameterVariable(Variable variable, Parameter parameter, string text)
            : base(parameter, text)
        {
            this.variable = variable;

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParameterWindow(parameter, variable.War3Type.Type);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandVariableModifyInitialValue command = new CommandVariableModifyInitialValue(variable, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
