using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class IfThenElse : TriggerElement, ICloneable
    {
        public readonly int ParamType = 5; // DO NOT CHANGE
        public List<TriggerElement> If = new List<TriggerElement>();
        public List<TriggerElement> Then = new List<TriggerElement>();
        public List<TriggerElement> Else = new List<TriggerElement>();
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
