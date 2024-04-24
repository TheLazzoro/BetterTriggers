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
            this.eca = eca;
            this.explorerElement = explorerElement;
            if (eca is ReturnStatement returnStatement)
            {
                switch (explorerElement.ElementType)
                {
                    case ExplorerElementEnum.ConditionDefinition:
                        this.returnType = "boolean";
                        break;
                    case ExplorerElementEnum.FunctionDefinition:
                        this.returnType = explorerElement.functionDefinition.ReturnType.War3Type.Type;
                        break;
                    default:
                        break;
                }
            }
            else
                this.returnType = returnType;

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var parameter = parameters[index];
            var window = new ParameterWindow(parameter, returnType, eca.function, explorerElement);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandTriggerElementParamModify command = new CommandTriggerElementParamModify(explorerElement, eca, parameters, index, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
