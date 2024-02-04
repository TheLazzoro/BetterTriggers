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
        List<Parameter_Saveable> GetParametersAll();
        Parameter_Saveable GetParameter(int index);
        void SetParameterAtIndex(Parameter_Saveable parameter, int index);
        List<string> GetReturnTypes();
        string GetParameterText();
    }
}
