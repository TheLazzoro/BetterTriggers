using System;

namespace BetterTriggers.Models.Templates
{
    public class ConstantTemplate : ParameterTemplate
    {
        public readonly int ParamType = 2; // DO NOT CHANGE
        public string codeText;

        public ConstantTemplate Clone()
        {
            ConstantTemplate clone = (ConstantTemplate)this.MemberwiseClone();
            clone.codeText = new string(codeText);
            clone.identifier = new string(identifier);
            clone.name = new string(name);
            clone.returnType = new string(returnType);

            return clone;
        }
    }
}
