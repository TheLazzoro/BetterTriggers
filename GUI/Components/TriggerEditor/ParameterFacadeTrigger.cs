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
        List<string> returnTypes;

        public ParameterFacadeTrigger(TreeViewTriggerElement treeItem, List<Parameter> parameters, List<string> returnTypes, string paramText)
        {
            this.treeItem = treeItem;
            this.parameters = parameters;
            this.paramText = paramText;
            this.returnTypes = returnTypes;
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
            return returnTypes;
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
