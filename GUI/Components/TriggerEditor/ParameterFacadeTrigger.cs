using BetterTriggers.Models.SaveableData;
using BetterTriggers.WorldEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class ParameterFacadeTrigger : IParameterFacade
    {
        private TreeViewTriggerElement treeItem;
        private List<Parameter> parameters;
        private string paramText;

        public ParameterFacadeTrigger(TreeViewTriggerElement treeItem, string paramText)
        {
            this.treeItem = treeItem;
            this.parameters = treeItem.triggerElement.function.parameters;
            this.paramText = paramText;
        }

        public TreeViewTriggerElement GetTreeItem()
        {
            return treeItem;
        }

        public Parameter GetParameter(int index)
        {
            return parameters[index];
        }

        public List<Parameter> GetParametersAll()
        {
            return parameters;
        }

        public List<string> GetReturnTypes()
        {
            return TriggerData.GetParameterReturnTypes(treeItem.triggerElement.function);
        }

        public void SetParameterAtIndex(Parameter parameter, int index)
        {
            parameters[index] = parameter;
        }

        public string GetParameterText()
        {
            return paramText;
        }
    }
}
