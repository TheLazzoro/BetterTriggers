using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class OrMultiple : Function, ICloneable
    {
        public readonly int ParamType = 7; // DO NOT CHANGE
        public List<TriggerElement> Or = new List<TriggerElement>();
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
