using Model.Data;
using System;
using System.Collections.Generic;

namespace Model.Natives
{
    public class Condition : ICloneable
    {
        public string identifier;
        public string displayName;
        public string paramText;
        public List<Parameter> parameters;
        public EnumCategory category;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
