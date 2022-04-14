using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class ForForceMultiple : Function, ICloneable
    {
        public readonly int ParamType = 9; // DO NOT CHANGE
        public List<TriggerElement> Actions = new List<TriggerElement>();
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
