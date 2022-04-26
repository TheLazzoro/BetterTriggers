using Model.EditorData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.SaveableData
{
    public class AndMultiple : Function
    {
        public readonly int ParamType = 6; // DO NOT CHANGE
        public List<TriggerElement> And = new List<TriggerElement>();
        
        public AndMultiple Clone()
        {
            AndMultiple andMultiple = new AndMultiple();
            andMultiple.identifier = new string(identifier);
            andMultiple.returnType = new string(returnType);
            andMultiple.And = new List<TriggerElement>();
            And.ForEach(element => andMultiple.And.Add(element.Clone()));

            return andMultiple;
        }
    }
}
