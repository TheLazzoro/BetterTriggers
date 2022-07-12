using BetterTriggers.Models.EditorData;
using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class ParameterFacadeVariable : IParameterFacade
    {
        private ExplorerElementVariable explorerElement;
        private string paramText;
        private List<string> returnTypes = new List<string>();

        public ParameterFacadeVariable(ExplorerElementVariable explorerElement, string paramText)
        {
            this.explorerElement = explorerElement;
            this.paramText = paramText;
            this.returnTypes.Add(explorerElement.variable.Type);
        }

        public ExplorerElementVariable GetExplorerElementVariable()
        {
            return explorerElement;
        }

        public Parameter GetParameter(int index)
        {
            return explorerElement.variable.InitialValue;
        }

        public List<Parameter> GetParametersAll()
        {
            List<Parameter> list = new List<Parameter>();
            list.Add(explorerElement.variable.InitialValue);
            return list;
        }

        public string GetParameterText()
        {
            return paramText;
        }

        public List<string> GetReturnTypes()
        {
            return returnTypes;
        }

        public void SetParameterAtIndex(Parameter parameter, int index)
        {
            explorerElement.variable.InitialValue = parameter;
        }
    }
}
