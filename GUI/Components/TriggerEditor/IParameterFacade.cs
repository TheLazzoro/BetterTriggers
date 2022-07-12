using BetterTriggers.Models.SaveableData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Components.TriggerEditor
{
    public interface IParameterFacade
    {
        List<Parameter> GetParametersAll();
        Parameter GetParameter(int index);
        void SetParameterAtIndex(Parameter parameter, int index);
        List<string> GetReturnTypes();
        string GetParameterText();
    }
}
