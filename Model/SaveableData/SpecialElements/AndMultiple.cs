using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class AndMultiple : Function, ICloneable
    {
        public readonly int ParamType = 6; // DO NOT CHANGE
        public List<TriggerElement> And = new List<TriggerElement>();
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
