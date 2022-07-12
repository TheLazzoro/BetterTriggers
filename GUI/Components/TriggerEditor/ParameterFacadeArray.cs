using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public class ParameterFacadeArray : IParameterFacade
    {
        private List<Parameter> parameters;
        private string paramText;
        private List<string> returnTypes;

        public ParameterFacadeArray(List<Parameter> parameters, string paramText)
        {
            this.parameters = parameters;
            this.paramText = paramText;
            this.returnTypes = new List<string>();
            this.returnTypes.Add("integer");
            this.returnTypes.Add("integer");
        }

        public Parameter GetParameter(int index)
        {
            return parameters[index];
        }

        public List<Parameter> GetParametersAll()
        {
            return parameters;
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
            parameters[index] = parameter;
        }
    }
}
