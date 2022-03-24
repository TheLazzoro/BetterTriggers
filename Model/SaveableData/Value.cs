using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public class Value : Parameter, ICloneable
    {
        public readonly int ParamType = 4; // DO NOT CHANGE

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
