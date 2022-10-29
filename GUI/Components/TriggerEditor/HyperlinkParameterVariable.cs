using BetterTriggers.Commands;
using BetterTriggers.Controllers;
using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using GUI.Controllers;
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
        internal ParameterFacadeVariable parameterFacade;
        Variable variable;

        public HyperlinkParameterVariable(ParameterFacadeVariable parameterFacade, string text)
            : base(parameterFacade.GetParameter(0), text)
        {
            this.parameterFacade = parameterFacade;
            this.variable = parameterFacade.GetVariable();

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var window = new ParameterWindow(parameter, variable.Type);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandVariableModifyInitialValue command = new CommandVariableModifyInitialValue(variable, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
