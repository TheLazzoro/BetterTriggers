using System;

namespace Model.Templates
{
    public class ConstantTemplate : ParameterTemplate
    {
        public readonly int ParamType = 2; // DO NOT CHANGE
        public string codeText;

        public ConstantTemplate Clone()
        {
            ConstantTemplate clone = (ConstantTemplate)this.MemberwiseClone();
            clone.codeText = string.Copy(codeText);
            clone.identifier = string.Copy(identifier);
            clone.name = string.Copy(name);
            clone.returnType = string.Copy(returnType);

            return clone;
        }
    }
}
