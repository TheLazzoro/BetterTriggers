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
        private List<Parameter_Saveable> parameters;
        private string paramText;
        List<string> returnTypes;

        public ParameterFacadeTrigger(TreeViewTriggerElement treeItem, List<Parameter_Saveable> parameters, List<string> returnTypes, string paramText)
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

        public Parameter_Saveable GetParameter(int index)
        {
            return parameters[index];
        }

        public List<Parameter_Saveable> GetParametersAll()
        {
            return parameters;
        }

        public List<string> GetReturnTypes()
        {
            return returnTypes;
        }

        public void SetParameterAtIndex(Parameter_Saveable parameter, int index)
        {
            parameters[index] = parameter;
        }

        public string GetParameterText()
        {
            return paramText;
        }
    }
}
