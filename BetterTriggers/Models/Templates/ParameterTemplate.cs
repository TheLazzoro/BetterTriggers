using BetterTriggers.Models.SaveableData;
using Newtonsoft.Json;
using System;

namespace BetterTriggers.Models.Templates
{
    public class ParameterTemplate
    {
        public string returnType;

        public virtual ParameterTemplate Clone()
        {
            ParameterTemplate clone = new ParameterTemplate();
            clone.returnType = new string(this.returnType);
            return clone;
        }

        public virtual Parameter_Saveable ToParameter()
        {
            return new Parameter_Saveable();
        }
    }
}
