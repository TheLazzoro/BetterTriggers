using System;

namespace BetterTriggers.Models.Templates
{
    public class ConstantTemplate : ParameterTemplate
    {
        public string value;
        public string name;
        public string codeText;

        public ConstantTemplate Clone()
        {
            ConstantTemplate clone = (ConstantTemplate)this.MemberwiseClone();
            clone.codeText = new string(codeText);
            clone.value = new string(value);
            clone.name = new string(name);
            clone.returnType = new string(returnType);

            return clone;
        }
    }
}
