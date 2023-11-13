using BetterTriggers.Commands;
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
    public class HyperlinkParameterTrigger : HyperlinkBT
    {
        internal ParameterFacadeTrigger parameterFacade;
        internal readonly int index;
        private TreeViewTriggerElement treeViewTriggerElement;
        private readonly string returnType;
        List<Parameter> parameters;


        public HyperlinkParameterTrigger(ParameterFacadeTrigger parameterFacade, string text, List<Parameter> parameters, int index, string returnType)
            : base(parameters[index], text)
        {
            this.parameterFacade = parameterFacade;
            this.parameters = parameters;
            this.index = index;
            this.returnType = returnType;
            this.treeViewTriggerElement = parameterFacade.GetTreeItem();

            this.Click += HyperlinkParameter_Click;
        }

        private void HyperlinkParameter_Click(object sender, RoutedEventArgs e)
        {
            var parameter = parameters[index];
            var triggerElement = (ECA)treeViewTriggerElement.triggerElement;
            var window = new ParameterWindow(parameter, returnType, triggerElement.function);
            window.ShowDialog();

            if (window.isOK) // set parameter on window close.
            {
                CommandTriggerElementParamModify command = new CommandTriggerElementParamModify(triggerElement, treeViewTriggerElement.GetExplorerElementTrigger(), parameters, index, window.selectedParameter);
                command.Execute();
            }
        }
    }
}
