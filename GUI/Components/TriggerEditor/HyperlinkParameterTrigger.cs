using BetterTriggers.Commands;
using BetterTriggers.Models.EditorData;
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
    public class HyperlinkParameterTrigger : HyperlinkBT
    {
        internal readonly int index;
        private ECA eca;
        private readonly string returnType;
        List<Parameter> parameters;
        ExplorerElement explorerElement;

        public HyperlinkParameterTrigger(ExplorerElement explorerElement, ECA eca, string text, List<Parameter> parameters, int index, string returnType)
            : base(parameters[index], text)
        {
            this.parameters = parameters;
            this.index = index;
            this.returnType = returnType;
            this.eca = eca;
            this.explorerElement = explorerElement;

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var parameter = parameters[index];
            var window = new ParameterWindow(parameter, returnType, eca.function, explorerElement.trigger);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandTriggerElementParamModify command = new CommandTriggerElementParamModify(explorerElement, eca, parameters, index, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
