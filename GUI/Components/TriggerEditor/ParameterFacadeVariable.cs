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
        private Variable variable;
        private string paramText;
        private List<string> returnTypes = new List<string>();

        public ParameterFacadeVariable(Variable variable, string paramText)
        {
            this.variable = variable;
            this.paramText = paramText;
            this.returnTypes.Add(variable.Type);
        }

        public Variable GetVariable()
        {
            return variable;
        }

        public Parameter GetParameter(int index)
        {
            return variable.InitialValue;
        }

        public List<Parameter> GetParametersAll()
        {
            List<Parameter> list = new List<Parameter>();
            list.Add(variable.InitialValue);
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
            variable.InitialValue = parameter;
        }
    }
}
