using Model.Data;
using System;
using System.Collections.Generic;

namespace Model.Natives
{
    public class Function : Parameter, ICloneable
    {
        public int ParamType = 1; // DO NOT CHANGE

        public List<Parameter> parameters = new List<Parameter>();
        public string paramText;
        public string description;
        public EnumCategory category;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
