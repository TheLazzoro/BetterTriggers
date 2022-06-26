using System.Collections.Generic;

namespace BetterTriggers.Models.SaveableData
{
    public class AndMultiple : Function
    {
        public readonly int ParamType = 6; // DO NOT CHANGE
        public List<TriggerElement> And = new List<TriggerElement>();
        
        public new AndMultiple Clone()
        {
            AndMultiple andMultiple = new AndMultiple();
            andMultiple.identifier = new string(identifier);
            andMultiple.returnType = new string(returnType);
            andMultiple.And = new List<TriggerElement>();
            And.ForEach(element => andMultiple.And.Add(element.Clone()));

            Function f = base.Clone();
            andMultiple.parameters = f.parameters;

            return andMultiple;
        }
    }
}
