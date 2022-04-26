using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class SetVariable : Function
    {
        public readonly int ParamType = 13; // DO NOT CHANGE
        
        public SetVariable Clone()
        {
            throw new NotImplementedException();
        }
    }
}
