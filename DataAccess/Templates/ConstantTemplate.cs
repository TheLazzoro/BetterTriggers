using System;

namespace Model.Templates
{
    public class ConstantTemplate : ParameterTemplate, ICloneable
    {
        public readonly int ParamType = 2; // DO NOT CHANGE
        public string codeText;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
