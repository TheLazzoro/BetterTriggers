using System;
using System.Collections.Generic;
using System.Text;

namespace Model.SaveableData
{
    public class Value : Parameter
    {
        public readonly int ParamType = 4; // DO NOT CHANGE

        public Value Clone()
        {
            return new Value()
            {
                identifier = new string(identifier),
                returnType = new string(returnType),
            };
        }
    }
}
