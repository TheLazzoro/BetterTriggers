using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    /// <summary>
    /// Things like 'Player00' or 'DestructableNull'
    /// </summary>
    public class Constant : Parameter
    {
        public readonly int ParamType = 2; // DO NOT CHANGE

        public Constant Clone()
        {
            return new Constant()
            {
                identifier = new string(identifier),
                returnType = new string(returnType),
            };
        }
    }
}
